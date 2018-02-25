using NRelay.Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NRelay.Server
{
    public class Connection : IDisposable
    {
        private int port;

        private byte[] incomingBuffer = new byte[4];
        private bool receivingMessage = false;

        private TcpClient socket;
        private NetworkStream stream;

        public bool Connected
        {
            get => socket?.Connected ?? false;
        }
        public event ConnectedToServerHandler ConnectedToServer;
        public event ServerErrorHandler Error;
        public event MessageHandler MessageReceived;

        public Connection(int port = 5680)
        {
            this.port = port;
        }

        public bool Connect()
        {
            if (!Connected)
            {
                StartConnection();
                return true;
            } else
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
            }
            if (socket != null)
            {
                socket.Close();
            }
        }

        public void Write(string message)
        {
            byte[] data;
            try
            {
                data = Encoding.UTF8.GetBytes(message);
            } catch (Exception e)
            {
                Error?.Invoke(this, new ServerErrorEventArgs(e));
                return;
            }
            try
            {
                stream.BeginWrite(data, 0, data.Length, (result) =>
                {
                    stream.EndWrite(result);
                }, null);
            } catch (Exception e)
            {
                Error?.Invoke(this, new ServerErrorEventArgs(e));
            }
        }

        private void StartConnection()
        {
            socket = new TcpClient();
            try
            {
                socket.BeginConnect("localhost", port, BeginConnectCallback, null);
            } catch (Exception e)
            {
                Error?.Invoke(this, new ServerErrorEventArgs(e));
            }
        }

        private void BeginConnectCallback(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);
            } catch (Exception e)
            {
                Error?.Invoke(this, new ServerErrorEventArgs(e));
            }
            if (socket.Connected)
            {
                ConnectedToServer?.Invoke(this, new ConnectedToServerEventArgs());
                try
                {
                    stream = socket.GetStream();
                    stream.BeginRead(incomingBuffer, 0, 4, ReadCallback, null);
                } catch (Exception e)
                {
                    Error?.Invoke(this, new ServerErrorEventArgs(e));
                }
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                stream.EndRead(result);
            } catch (Exception e)
            {
                Error?.Invoke(this, new ServerErrorEventArgs(e));
            }
            if (!receivingMessage)
            {
                int messageLength;
                try
                {
                    messageLength = BitConverter.ToInt32(incomingBuffer, 0);
                } catch (Exception e)
                {
                    Error?.Invoke(this, new ServerErrorEventArgs(e));
                    return;
                }
                receivingMessage = true;
                incomingBuffer = new byte[messageLength];
                try
                {
                    stream.BeginRead(incomingBuffer, 0, messageLength, ReadCallback, null);
                } catch (Exception e)
                {
                    Error?.Invoke(this, new ServerErrorEventArgs(e));
                }
                
            } else
            {
                string message;
                try
                {
                    message = Encoding.UTF8.GetString(incomingBuffer);
                } catch (Exception e)
                {
                    Error?.Invoke(this, new ServerErrorEventArgs(e));
                    return;
                }
                incomingBuffer = new byte[4];
                receivingMessage = false;
                MessageReceived?.Invoke(this, new MessageEventArgs(message));
                try
                {
                    stream.BeginRead(incomingBuffer, 0, 4, ReadCallback, null);
                } catch (Exception e)
                {
                    Error?.Invoke(this, new ServerErrorEventArgs(e));
                }
            }
        }
    }
}
