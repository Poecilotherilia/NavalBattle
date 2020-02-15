using ProtoBuf;
using protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace game
{
    public class SocketClient
    {
        private Socket mSocket;
        private Thread mSendThread = null;
        private Thread mRecvThread = null;

        private volatile bool mIsRunning = false;

        // 发送
        private object mSendLock = null;
        private Queue<NetMsgData> mSendingMsgQueue = null;
        private Queue<NetMsgData> mSendWaitingMsgQueue = null;
        // 接收
        private object mRecvLock = null;
        private Queue<NetMsgData> mRecvingMsgQueue = null;
        private Queue<NetMsgData> mRecvWaitingMsgQueue = null;

        private static SocketClient instance;

        public static SocketClient Instance {
            get { return GetInstance(); }
        }

        public static SocketClient GetInstance()
        {
            if (null == game.SocketClient.instance)
            {
                game.SocketClient.instance = Activator.CreateInstance<SocketClient>();
            }
            return game.SocketClient.instance;
        }

        string ip = "115.159.221.30";
        //string ip = "192.168.1.3";
        int port = 6666;

        NetworkStream stream;
        TcpClient client;
        byte[] buffer;
        int BuffseSize = 8192;

        public void SocketClientConnect()
        {
            client = new TcpClient();
            client.Connect(ip, port);

            Debug.Log("连接服务器 -> " + client.Client.RemoteEndPoint);
            this.mRecvingMsgQueue = new Queue<NetMsgData>();
            stream = client.GetStream();
            buffer = new byte[8192];
            NetManager.Instance.ReqHandshake();
            stream.BeginRead(buffer, 0, buffer.Length, ReadAsync, null);
            UpdateManager.Instance.AddToUpdate(typeof(SocketClient).ToString(), this.ReceiveUpdate);
        }

        void ReceiveUpdate() {
            if(this.mRecvingMsgQueue.Count>0)
            NetManager.DispatcherMsg(this.mRecvingMsgQueue.Dequeue());
        }

        void ReadAsync(IAsyncResult result)
        {
            try
            {
                int readCount = stream.EndRead(result);
                if (readCount == 0) throw new Exception("读取到0字节");
                NetMsgData receive = NetUtilcs.UnpackNetMsg(buffer);
                this.mRecvingMsgQueue.Enqueue(receive);
                //string msg = Encoding.UTF8.GetString(buffer, 0, readCount);
                Debug.Log("接收到消息 -> " + receive.ProtoId);

                lock (stream) //再次开启读取
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.BeginRead(buffer, 0, BuffseSize, ReadAsync, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //发送消息到服务器
        public void SendMsg(ProtoDefine protoType, IExtensible protoData)
        {
            //byte[] temp = Encoding.UTF8.GetBytes(msg);
            NetMsgData msg = NetMsgData.GetMsgData(protoType, protoData);
            byte[] data = NetUtilcs.PackNetMsg(msg);
            stream.Write(data, 0, data.Length);
            Debug.Log("发送消息 -> " + msg.ProtoId+data);
        }


        public void Connect(string host, int port)
        {
            if (this.mIsRunning)
                return;

            if (string.IsNullOrEmpty(host))
            {
                Debug.LogError("NetMgr.Connect host is null");
                return;
            }

            IPEndPoint ipEndPoint = null;
            Regex regex = new Regex("((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)\\.){3}(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|[1-9])");
            Match match = regex.Match(host);
            if (match.Success)
            {
                // IP
                ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            }
            else
            {
                // 域名
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                ipEndPoint = new IPEndPoint(addresses[0], port);
            }

            //新建连接，连接类型
            mSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                mSocket.Connect(ipEndPoint);//链接IP和端口
                this.mIsRunning = true;
                InitTreadAndQueue();
                NetManager.Instance.ReqHandshake();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Disconnect();
            }
        }

        void InitTreadAndQueue()
        {
            this.mSendLock = new object();
            this.mSendingMsgQueue = new Queue<NetMsgData>();
            this.mSendWaitingMsgQueue = new Queue<NetMsgData>();
            this.mRecvLock = new object();
            this.mRecvingMsgQueue = new Queue<NetMsgData>();
            this.mRecvWaitingMsgQueue = new Queue<NetMsgData>();

            this.mSendThread = new Thread(new ThreadStart(Send));
            this.mRecvThread = new Thread(new ThreadStart(Receive));
            this.mSendThread.Start();
            this.mRecvThread.Start();
        }

        void Send()
        {
            while (this.mIsRunning)
            {
                if (mSendingMsgQueue.Count == 0)
                {
                    lock (this.mSendLock)
                    {
                        while (this.mSendWaitingMsgQueue.Count == 0)
                            Monitor.Wait(this.mSendLock);
                        Queue<NetMsgData> temp = this.mSendingMsgQueue;
                        this.mSendingMsgQueue = this.mSendWaitingMsgQueue;
                        this.mSendWaitingMsgQueue = temp;
                    }
                }
                else
                {
                    try
                    {
                        NetMsgData msg = this.mSendingMsgQueue.Dequeue();
                        byte[] data = NetUtilcs.PackNetMsg(msg);
                        mSocket.Send(data, data.Length, SocketFlags.None);
                        Debug.Log("client send: " + (ProtoDefine)msg.ProtoId);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                        Disconnect();
                    }
                }
            }

            this.mSendingMsgQueue.Clear();
            this.mSendWaitingMsgQueue.Clear();
        }

        public void SendMsg1(ProtoDefine protoType, IExtensible protoData)
        {
            if (!this.mIsRunning) return;
            lock (this.mSendLock)
            {
                mSendWaitingMsgQueue.Enqueue(NetMsgData.GetMsgData(protoType, protoData));
                Monitor.Pulse(this.mSendLock);
            }
        }

        void Receive()
        {
            byte[] data = new byte[1024];
            while (this.mIsRunning)
            {
                try
                {
                    //将收到的数据取出来
                    int len = mSocket.Receive(data);
                    NetMsgData receive = NetUtilcs.UnpackNetMsg(data);
                    Debug.Log("client receive : " + (ProtoDefine)receive.ProtoId);

                    lock (this.mRecvLock)
                    {
                        this.mRecvWaitingMsgQueue.Enqueue(receive);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    Disconnect();
                }

            }
        }

        public void Update()
        {
            if (!this.mIsRunning) return;

            if (this.mRecvingMsgQueue.Count == 0)
            {
                lock (this.mRecvLock)
                {
                    if (this.mRecvWaitingMsgQueue.Count > 0)
                    {
                        Queue<NetMsgData> temp = this.mRecvingMsgQueue;
                        this.mRecvingMsgQueue = this.mRecvWaitingMsgQueue;
                        this.mRecvWaitingMsgQueue = temp;
                    }
                }
            }
            else
            {
                while (this.mRecvingMsgQueue.Count > 0)
                {
                    NetMsgData msg = this.mRecvingMsgQueue.Dequeue();
                    //发送给逻辑处理
                    NetManager.DispatcherMsg(msg);
                }
            }
        }

        public void Disconnect()
        {
            //if (!this.mIsRunning)
            ///    return;
            try
            {
                if (this.mSocket.Available != 0)
                    this.mSocket.Shutdown(SocketShutdown.Both);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                this.mIsRunning = false;
                this.mSocket.Close();
            }
        }

        public void OnDestroy()
        {
            //Disconnect();
            //this.mIsRunning = false;
            try
            {
                this.stream.Close();
                this.client.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}