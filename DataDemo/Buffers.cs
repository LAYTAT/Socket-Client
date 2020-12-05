using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace Buff
{

    public class MsgInfo
    {
        private Int32 msgID;
        private Int32 uID;
        private Int32 pacLen;
        private Byte[] payload;

        public MsgInfo(Int32 msgID, Int32 uID, Int32 pacLen)
        {
            this.msgID = msgID;
            this.uID = uID;
            this.pacLen = pacLen;
        }

        public Int32 GetMsgID()
        {
            return msgID;
        }

        public Int32 GetUID()
        {
            return uID;
        }

        public Int32 GetPacLen()
        {
            return pacLen;
        }
        public Byte[] GetPayload()
        {
            return payload;
        }

        public void SetPayload(Byte[] pld)
        {
            payload = pld;
        }
    }
    public class Buffers
    {
        public const int SIZE = 1024 * 1024;
        private int buffTempLen;
        private Byte[] buffTemp;
        public Byte[] buff;
        private int start;
        private int end;
        private int size;


        public Buffers()
        {
            start = end = 0;
        }
        public void Init(int tempLen, int len = SIZE)
        {
            size = len;
            buff = new Byte[size];
            buffTempLen = tempLen;
            buffTemp = new Byte[tempLen];
        }
        void Reset()
        {
            start = end = buffTempLen = 0;
        }

        public bool IsEmpty()
        {
            return start == end;
        }
        public int GetLeftSize()
        {
            return size - 1 - ((end - start + size) % size);
        }

        public Byte[] GetBuff()
        {
            return buff;
        }
        public int GetWriteContinuedSize()
        {
            return Math.Min(GetLeftSize(), size - ((1 + end)%size));
        }
        public int GetStart()
        {
            return start;
        }

        public int GetEnd()
        {
            return end;
        }

        public int GetNextWritableEnd()
        {
            return (end + 1) % size;
        }

        public int GetReadableStart()
        {
            return (start + 1) % size;
        }

        public int GetDataSize()
        {
            return (end - start + size) % size;
        }

        public void SetEndByLen(int len)
        {
            end = (end + len) % size;
        }

        public void SetStartByLen(int len)
        {
            start = (start + len) % size;
        }
        public bool IsHeaderReadable(int size)  // 3 sizeof(INT32)
        {
            return GetDataSize() >= size;
        }

        public bool IsPayloadReadable(int size)
        {
            return GetDataSize() >= size;
        }

        public bool IsHeaderAndPayloadReadable(int size)
        {
            return GetDataSize() >= size;
        }
        public void GetPayload(MsgInfo msgInfo)
        {
            SetStartByLen(3*sizeof(Int32));
            Byte[] payloadTemp = new byte[msgInfo.GetPacLen()];
            Byte[] buffTemp = GetBuff();
            
            int startTemp = GetStart();
            for (int i = 0; i < msgInfo.GetPacLen(); i++)
            {
                payloadTemp[i] = buffTemp[(startTemp + 1 + i) % size];
            }
            msgInfo.SetPayload(payloadTemp);
            SetStartByLen(msgInfo.GetPacLen());

        }

        public static Int32 Encode(Byte[] buffer, int msgID, int uID, int pacLen)
        {
            int index = 0;
            Int32 tmp = IPAddress.HostToNetworkOrder(msgID);
            foreach (Byte b in BitConverter.GetBytes(tmp))
            {
                buffer[index] = b;
                index++;
            }


            tmp = IPAddress.HostToNetworkOrder(uID);
            foreach (Byte b in BitConverter.GetBytes(tmp))
            {
                buffer[index] = b;
                index++;
            }

            tmp = IPAddress.HostToNetworkOrder(pacLen);
            foreach (Byte b in BitConverter.GetBytes(tmp))
            {
                buffer[index] = b;
                index++;
            }
            return 3 * sizeof(Int32);
        }

        public MsgInfo Decode()
        {
            UInt32 msgID;
            UInt32 uID;
            UInt32 pacLen;
            int offset = GetReadableStart();

            // msgID = BitConverter.ToUInt32(GetBuff(), offset);
            // temp = BitConverter.GetBytes(msgID);
            Byte[] temp = GetBuff();
            Byte[] t = new byte[sizeof(Int32)];


            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    t[i] = temp[(t.Length - i + offset - 1)%size];
                }
                offset += sizeof(Int32);
                msgID = BitConverter.ToUInt32(t, 0);
                for (int i = 0; i < t.Length; i++)
                {
                    t[i] = temp[(t.Length - i + offset - 1) % size];
                }
                uID = BitConverter.ToUInt32(t, 0);
                offset += sizeof(Int32);

                for (int i = 0; i < t.Length; i++)
                {
                    t[i] = temp[(t.Length - i + offset - 1) % size];
                }
                pacLen = BitConverter.ToUInt32(t, 0);
            }
            else
            {
                msgID = BitConverter.ToUInt32(temp, offset);
                uID = BitConverter.ToUInt32(temp, offset+sizeof(Int32));
                pacLen = BitConverter.ToUInt32(temp, offset + sizeof(Int32)*2);
            }
            
            // SetStartByLen(3 * sizeof(Int32));
            return new MsgInfo((Int32)msgID, (Int32)uID, (Int32)pacLen);
        }

    }
}
