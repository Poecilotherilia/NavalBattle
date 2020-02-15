using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol;
using System;
using ProtoBuf;
using UnityEngine.Events;

namespace game
{
    public enum HandleShakeState
    {
        CLIENT_CALL_SERVER, //等待客户端请求验证
        SERVER_REPLY_CLIENT, //等待服务端回复验证
        CLIENT_REPLY_SERVER, //等待客户端回复验证
        HANDLESHAKE_SUCCEED, //握手成功
    }
    public delegate void NetCallBack(IExtensible msgData);
    public class NetManager : MonoBehaviour
    {
        private static NetManager _instance;
        public static NetManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(NetManager)) as NetManager;
                }
                return _instance;
            }
        }

        private static Dictionary<ProtoDefine, Delegate> m_EventTable = new Dictionary<ProtoDefine, Delegate>();
        private HandleShakeState handleShakeState = HandleShakeState.CLIENT_CALL_SERVER;

        void Awake()
        {
            //SocketClint.Instance.Connect("127.0.0.1", 6666);
            SocketClient.Instance.SocketClientConnect();
            this.Initialize();

        }

        void Start() {
            //UpdateManager.Instance.AddToUpdate(typeof(NetManager).ToString(),
             //   SocketClint.Instance.Update);
        }

        public void Initialize()
        {
            NetManager.ListenerMsg(ProtoDefine.Handshake, HandshakeCallBack);

            handleShakeState = HandleShakeState.CLIENT_CALL_SERVER;
        }

        public void ReqHandshake()
        {
            Handshake msgData = new Handshake
            {
                token = NetUtilcs.HANDLESHAKE_1
            };
            NetManager.SendMsg(ProtoDefine.Handshake, msgData);
        }

        private void HandshakeCallBack(IExtensible msgData)
        {
            Handshake handshake = (Handshake)msgData;
            string token = handshake.token;
            Debug.Log("HandshakeCallBack" + token);
            if (token == NetUtilcs.HANDLESHAKE_2)
            {
                Handshake reponse = new Handshake
                {
                    token = NetUtilcs.HANDLESHAKE_3
                };
                NetManager.SendMsg(ProtoDefine.Handshake, reponse);
                handleShakeState = HandleShakeState.SERVER_REPLY_CLIENT;
            }
            else if (token == NetUtilcs.HANDLESHAKE_4)
            {
                //验证结束，请求登录服务器
                handleShakeState = HandleShakeState.HANDLESHAKE_SUCCEED;
                Debug.Log("握手成功");
            }
            else
            {
                Debug.LogError("握手失败，非目标服务");
            }
        }

        public static void ListenerMsg(ProtoDefine protoType, NetCallBack callBack)
        {
            if (!m_EventTable.ContainsKey(protoType))
            {
                m_EventTable.Add(protoType, null);
            }

            m_EventTable[protoType] = (NetCallBack)m_EventTable[protoType] + callBack;
        }

        public static void RemoveListenerMsg(ProtoDefine protoType, NetCallBack callBack)
        {
            if (m_EventTable.ContainsKey(protoType))
            {
                m_EventTable[protoType] = (NetCallBack)m_EventTable[protoType] - callBack;

                if (m_EventTable[protoType] == null)
                {
                    m_EventTable.Remove(protoType);
                }
            }
        }

        public static void DispatcherMsg(NetMsgData msgData)
        {
            ProtoDefine protoType = (ProtoDefine)msgData.ProtoId;
            Delegate d;
            if (m_EventTable.TryGetValue(protoType, out d))
            {
                NetCallBack callBack = d as NetCallBack;
                if (callBack != null)
                {
                    callBack(msgData.ProtoData);
                }
            }
        }

        public static void SendMsg(ProtoDefine protoType, IExtensible protoData)
        {
            SocketClient.Instance.SendMsg(protoType, protoData);
        }

        public void ReqCloseConnect()
        {
            CloseConnect msgData = new CloseConnect
            {

            };
            NetManager.SendMsg(ProtoDefine.CloseConnect, msgData);
        }

        void OnDestroy() {
            ReqCloseConnect();
            SocketClient.Instance.OnDestroy();
        }
    }
}