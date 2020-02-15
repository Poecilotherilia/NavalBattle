using System;
using System.IO;
using ProtoBuf;
using protocol;
using UnityEngine;

namespace game
{
    public class NetMsgData
    {
        public ushort ProtoId { get; set; }
        public ushort MsgLength { get; set; }
        public IExtensible ProtoData { get; set; }

        public NetMsgData(ushort protoId, ushort msgLength, IExtensible protodata)
        {
            ProtoId = protoId;
            MsgLength = msgLength;
            ProtoData = protodata;
        }

        public NetMsgData() { }

        public void Reset()
        {
            ProtoId = 0;
            MsgLength = 0;
            ProtoData = null;
        }
        public static NetMsgData GetMsgData(ProtoDefine protoType, IExtensible protoData, ushort length = 0)
        {
            NetMsgData data = new NetMsgData();

            data.ProtoId = (ushort)protoType;
            data.MsgLength = length;
            data.ProtoData = protoData;
            return data;
        }
    }
    public class NetUtilcs
    {
        public static readonly string HANDLESHAKE_1 = "This is the client. Can you hear me";
        public static readonly string HANDLESHAKE_2 = "This is the server. I can hear you. Can you hear me";
        public static readonly string HANDLESHAKE_3 = "I'm the client. I can hear you";
        public static readonly string HANDLESHAKE_4 = "I'm the server. you can login now";

        /// <summary>  
        /// 序列化  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="msg"></param>  
        /// <returns></returns>  
        static public byte[] Serialize<T>(T msg)
        {
            byte[] result = null;
            if (msg != null)
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize<T>(stream, msg);
                    result = stream.ToArray();
                }
            }
            return result;
        }

        public static byte[] PackNetMsg(NetMsgData data)
        {
            ushort protoId = data.ProtoId;
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                byte[] pbdata = Serialize(data.ProtoData);
                ushort msglen = (ushort)pbdata.Length;
                writer.Write(msglen);
                writer.Write(protoId);
                writer.Write(pbdata);
                writer.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>  
        /// 反序列化  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        static public T Deserialize<T>(byte[] message)
        {
            T result = default(T);
            if (message != null)
            {
                using (var stream = new MemoryStream(message))
                {
                    result = Serializer.Deserialize<T>(stream);
                }
            }
            return result;
        }

        public static NetMsgData UnpackNetMsg(byte[] msgData)
        {
            MemoryStream ms = null;

            using (ms = new MemoryStream(msgData))
            {
                BinaryReader reader = new BinaryReader(ms);
                ushort msgLen = reader.ReadUInt16();
                ushort protoId = reader.ReadUInt16();

                if (msgLen <= msgData.Length - 4)
                {
                    IExtensible protoData = CreateProtoBuf.GetProtoData((ProtoDefine)protoId, reader.ReadBytes(msgLen));
                    return NetMsgData.GetMsgData((ProtoDefine)protoId, protoData, msgLen);
                }
                else
                {
                    Debug.LogError("协议长度错误");
                }
            }

            return null;
        }
    }
}
