using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace SocketAsync
{
    public class SocketServer
    {
        TcpListener mTCPListener;
        IPAddress mIP;
        int mPort;

        List<TcpClient> mClients;
        public bool keeprunning { get; set; }
        public bool error_with_connection { get; private set; }

        public SocketServer()
        {
            mClients = new List<TcpClient>();
        }
        public async void StartListeningForIncomingConnection(IPAddress ipaddr = null,int port = 502)
        {
            if (ipaddr == null)
                ipaddr = IPAddress.Any;
            if (port <= 0)
                port = 502;

            mIP = ipaddr;
            mPort = port;
            Debug.WriteLine($"IPAddress: {mIP} || Port: {mPort}");

            //create a listener class to help us create server 
            mTCPListener = new TcpListener(mIP, mPort);

            try
            {
                mTCPListener.Start();
                keeprunning = true;
                while (keeprunning)
                {
                    //accept clients 
                    var returnedClient = await mTCPListener.AcceptTcpClientAsync();
                    mClients.Add(returnedClient);
                    Debug.WriteLine($"{mClients.Count} Client Connected | {returnedClient.Client.RemoteEndPoint}");
                    TakeCareOfTcpClient(returnedClient);
                }

            }
            catch(SocketException soc_ex)
            {
                Debug.WriteLine(soc_ex.Message);
            }
        }

        public void StopServer()
        {
            try
            {
                if(mTCPListener != null)
                   mTCPListener.Stop();

                foreach(TcpClient c in mClients)
                {
                    c.Close();
                }

                mClients.Clear();//sanity check
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void TakeCareOfTcpClient(TcpClient tcpClient)
        {
            NetworkStream ns = null;
            StreamReader sr = null;
            try
            {
                ns = tcpClient.GetStream();
                sr = new StreamReader(ns);

                //read data from the client
                char[] buff = new char[64];

                while (keeprunning)
                {                    
                    int nRet = await sr.ReadAsync(buff, 0, buff.Length);
                    if (nRet == 0)
                    {
                        RemoveClient(tcpClient);
                        Debug.WriteLine("***Socket Disconnected***");
                        break;
                    }
                        
                    Debug.WriteLine($"nRet:= { nRet}");
                    string recv = new string(buff);
                    Debug.WriteLine("**Reieved data");

                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (SocketException socex)
            {
                error_with_connection = true;
                Debug.WriteLine(socex.Message + Environment.NewLine + "**Socket exception Thrown**");
            }
            catch (Exception ex)
            {
                error_with_connection = true;
                Debug.WriteLine(ex.Message + Environment.NewLine + "**Problem with the connection!**");
            }
            finally
            {
                if(error_with_connection)
                    RemoveClient(tcpClient);
            }
        }

        private void RemoveClient(TcpClient tcpClient)
        {
            if (mClients.Contains(tcpClient))
            {
                mClients.Remove(tcpClient);
                Debug.WriteLine($"Client removed:= {tcpClient.Client.RemoteEndPoint} | Connected Clients:= {mClients.Count}");
            }

            error_with_connection = false;
        }

        public async void SendToAll(string msg)
        {
            if (String.IsNullOrEmpty(msg))
                return;

            try
            {
                byte[] buffMsg = Encoding.ASCII.GetBytes(msg);
                foreach(TcpClient c in mClients)
                {
                    c.GetStream().WriteAsync(buffMsg, 0, buffMsg.Length); //dont need await yet
                }
            }
            catch ( Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
