using System;
using System.Net;
using System.Net.Sockets;
using Buff;

namespace TCPSocket
{
    public class TCPSocketClient
    {
        // 创建终结点EndPoint
        private IPAddress ip;
        //把ip和端口转化为IPEndpoint实例
        private IPEndPoint ipEndPoint; 
        // 创建socket并连接到服务器
        private Socket socket;
        Byte[] RecvBuff;

        public TCPSocketClient(string strIP, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1); //1ms的阻塞时间
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                IPAddress ip = IPAddress.Parse(strIP);
                ipEndPoint = new IPEndPoint(ip, port);//把ip和端口转化为IPEndpoint实例
                socket.Connect(ipEndPoint);
                RecvBuff = new Byte[1024*1024];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public int Send(Byte[] data, int offset, int len, out SocketError errorCode, SocketFlags socketFlags = SocketFlags.None)
        {
            try
            {
                return socket.Send(data, offset, len, socketFlags, out errorCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                errorCode = SocketError.SocketError;
                return -1;
            }
        }

        public bool Connected()
        {
            return socket.Connected;
        }
        public int Receive(Buffers buff, out SocketError socketError, SocketFlags socketFlags=SocketFlags.None)
        {
            try
            {
               int ret = socket.Receive(buff.GetBuff(), buff.GetNextWritableEnd(), buff.GetWriteContinuedSize(),
                    socketFlags, out socketError);
               if (ret > 0)
               {
                   buff.SetEndByLen(ret);
               }

               return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                socketError = SocketError.SocketError;
                return -1;
            }
        }

    }
}