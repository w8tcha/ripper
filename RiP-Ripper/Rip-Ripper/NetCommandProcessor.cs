// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCommandProcessor.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   //  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: RiP-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RiPRipper
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The Net Command Processor.
    /// </summary>
    public class NetCommandProcessor
    {
        // Attributes
        private readonly ArrayList mClients = new ArrayList(); // Client Connections

        public int mListeningPort;

        private Socket mListenerSocket;

        private MainForm mMainForm;

        /// <summary>
        /// Starts the listening.
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        /// <param name="listeningPort">The listening port.</param>
        public void StartListening(MainForm mainForm, int listeningPort)
        {
            this.mMainForm = mainForm;
            this.mListeningPort = listeningPort;

            // Create the listener socket in this machines IP address
            this.mListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // mListenerSocket.Bind( new IPEndPoint( aryLocalAddr[0], mListeningPort ) );
                this.mListenerSocket.Bind(new IPEndPoint(IPAddress.Loopback, this.mListeningPort));
                    
                // For use with localhost 127.0.0.1
                this.mListenerSocket.Listen(10);
            }
            catch (SocketException)
            {
                // Catching this exception does not work, I'll look into it later
                System.Windows.Forms.MessageBox.Show(
                    string.Format("Another program is already listening on port {0}.", listeningPort));
                return;
            }

            Console.WriteLine("Listening for commands on port: " + this.mListeningPort);

            // Setup a callback to be notified of connection requests
            this.mListenerSocket.BeginAccept(this.OnConnectRequest, this.mListenerSocket);
        }

        /// <summary>
        /// Clears the listening socket.
        /// </summary>
        public void ClearListeningSocket()
        {
        }

        /// <summary>
        /// Stops the listening.
        /// </summary>
        public void StopListening()
        {
        }

        /// <summary>
        /// Callback used when a client requests a connection.
        /// Accpet the connection, adding it to our list and setup to
        /// accept more connections.
        /// </summary>
        /// <param name="ar">The ar.</param>
        public void OnConnectRequest(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            this.NewConnection(listener.EndAccept(ar));
            listener.BeginAccept(this.OnConnectRequest, listener);
        }

        /// <summary>
        /// Add the given connection to our list of clients
        /// Reply that connection was successful
        /// Setup a callback to recieve data
        /// </summary>
        /// <param name="sockClient">Connection to keep</param>
        public void NewConnection(Socket sockClient)
        {
            // Program blocks on Accept() until a client connects.
            SocketChatClient client = new SocketChatClient(sockClient);
            this.mClients.Add(client);
            Console.WriteLine("Client {0}, connected", client.Sock.RemoteEndPoint);

            const string strReply = "DONE";

            // Convert to byte array and send.
            byte[] byteReply = Encoding.ASCII.GetBytes(strReply.ToCharArray());
            client.Sock.Send(byteReply, byteReply.Length, 0);

            client.SetupRecieveCallback(this);
        }

        /// <summary>
        /// Receive data from the socket and process it.
        /// Note: If not data was recieved the connection has probably
        /// died.
        /// </summary>
        /// <param name="ar">The ar.</param>
        public void OnRecievedData(IAsyncResult ar)
        {
            SocketChatClient client = (SocketChatClient)ar.AsyncState;
            byte[] aryReceivedData = client.GetRecievedData(ar);

            // If no data was recieved then the connection is probably dead
            if (aryReceivedData.Length < 1)
            {
                Console.WriteLine("Client {0}, disconnected", client.Sock.RemoteEndPoint);
                client.Sock.Close();
                this.mClients.Remove(client);
                return;
            }

            string sReceivedData = Encoding.ASCII.GetString(aryReceivedData, 0, aryReceivedData.Length);

            int urlStart = sReceivedData.IndexOf("http://");

            if (urlStart != -1)
            {
                this.mMainForm.ProcessCommand(sReceivedData.Substring(urlStart));
            }

            client.SetupRecieveCallback(this);
        }
    }

    /// <summary>
    /// Class holding information and buffers for the Client socket connection
    /// </summary>
    internal class SocketChatClient
    {
        private readonly Socket mSock; // Connection to the client

        private readonly byte[] mByBuff = new byte[2000]; // Receive buffer (big until no longer doing HTTP com)

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sock">client socket conneciton this object represents</param>
        public SocketChatClient(Socket sock)
        {
            this.mSock = sock;
        }

        // Readonly access
        public Socket Sock
        {
            get
            {
                return this.mSock;
            }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="aNcp"></param>
        public void SetupRecieveCallback(NetCommandProcessor aNcp)
        {
            try
            {
                AsyncCallback recieveData = aNcp.OnRecievedData;
                this.mSock.BeginReceive(this.mByBuff, 0, this.mByBuff.Length, SocketFlags.None, recieveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = this.mSock.EndReceive(ar);
            }
            catch
            {
            }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(this.mByBuff, byReturn, nBytesRec);

            /*
            // Check for any remaining data and display it
            // This will improve performance for large packets 
            // but adds nothing to readability and is not essential
            int nToBeRead = mSock.Available;
            if( nToBeRead > 0 )
            {
                byte [] byData = new byte[nToBeRead];
                mSock.Receive( byData );
                // Append byData to byReturn here
            }
            */
            return byReturn;
        }
    }
}