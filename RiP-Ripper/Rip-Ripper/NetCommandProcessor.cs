
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace RiPRipper
{
	/// <summary>
	/// Summary description for NetCommandProcessor.
	/// </summary>
	public class NetCommandProcessor
	{
		// Attributes
		private readonly ArrayList	mClients = new ArrayList();	// Client Connections
		public		int			mListeningPort;
		private		Socket		mListenerSocket;
		private		MainForm		mMainForm;


	    public void StartListening(MainForm aMainForm, int aListeningPort )
		{
			mMainForm = aMainForm;
			mListeningPort = aListeningPort;

			// Create the listener socket in this machines IP address
			mListenerSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			
			try
			{
                //mListenerSocket.Bind( new IPEndPoint( aryLocalAddr[0], mListeningPort ) );
                mListenerSocket.Bind(new IPEndPoint(IPAddress.Loopback, mListeningPort));	// For use with localhost 127.0.0.1

				mListenerSocket.Listen( 10 );
			}

			// Catching this exception does not work, I'll look into it later
			catch( SocketException )
			{
				System.Windows.Forms.MessageBox.Show( "Another program is already listening on port " + aListeningPort + "." );
				return;
			}

			Console.WriteLine( "Listening for commands on port: " + mListeningPort );

			// Setup a callback to be notified of connection requests
			mListenerSocket.BeginAccept( new AsyncCallback( OnConnectRequest ), mListenerSocket );
			
		}


		public void ClearListeningSocket()
		{
		}


		public void StopListening()
		{
		}


		/// <summary>
		/// Callback used when a client requests a connection. 
		/// Accpet the connection, adding it to our list and setup to 
		/// accept more connections.
		/// </summary>
		/// <param name="ar"></param>
		public void OnConnectRequest( IAsyncResult ar )
		{

			Socket listener = (Socket)ar.AsyncState;
			NewConnection( listener.EndAccept( ar ) );
			listener.BeginAccept( new AsyncCallback( OnConnectRequest ), listener );
		}

		/// <summary>
		/// Add the given connection to our list of clients
		/// Reply that connection was successful
		/// Setup a callback to recieve data
		/// </summary>
		/// <param name="sockClient">Connection to keep</param>
		//public void NewConnection( TcpListener listener )
		public void NewConnection( Socket sockClient )
		{

			// Program blocks on Accept() until a client connects.
			SocketChatClient client = new SocketChatClient( sockClient );
			mClients.Add( client );
			Console.WriteLine( "Client {0}, connected", client.Sock.RemoteEndPoint );
 
			const string strReply = "DONE";

			// Convert to byte array and send.
			Byte[] byteReply = Encoding.ASCII.GetBytes( strReply.ToCharArray() );
			client.Sock.Send( byteReply, byteReply.Length, 0 );

			client.SetupRecieveCallback( this );
		}



		/// <summary>
		/// Receive data from the socket and process it. 
		/// Note: If not data was recieved the connection has probably 
		/// died.
		/// </summary>
		/// <param name="ar"></param>
		public void OnRecievedData( IAsyncResult ar )
		{

			SocketChatClient client = ( SocketChatClient )ar.AsyncState;
			byte [] aryReceivedData = client.GetRecievedData( ar );

			// If no data was recieved then the connection is probably dead
			if( aryReceivedData.Length < 1 )
			{
				Console.WriteLine( "Client {0}, disconnected", client.Sock.RemoteEndPoint );
				client.Sock.Close();
				mClients.Remove( client );      				
				return;
			}

			string sReceivedData = Encoding.ASCII.GetString( aryReceivedData, 0, aryReceivedData.Length );

			int urlStart = sReceivedData.IndexOf( "http://" );

			if( urlStart != -1 )
			{
				mMainForm.ProcessCommand( sReceivedData.Substring( urlStart ) );
			}


			client.SetupRecieveCallback( this );
		}
	}

	/// <summary>
	/// Class holding information and buffers for the Client socket connection
	/// </summary>
	internal class SocketChatClient
	{
		private readonly Socket mSock;							// Connection to the client
		private readonly byte[] mByBuff = new byte[2000];		// Receive buffer (big until no longer doing HTTP com)

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sock">client socket conneciton this object represents</param>
		public SocketChatClient( Socket sock )
		{
			mSock = sock;
		}

		// Readonly access
		public Socket Sock
		{
			get{ return mSock; }
		}

		/// <summary>
		/// Setup the callback for recieved data and loss of conneciton
		/// </summary>
        /// <param name="aNcp"></param>
		public void SetupRecieveCallback( NetCommandProcessor aNcp )
		{
			try
			{
				AsyncCallback recieveData = aNcp.OnRecievedData;
				mSock.BeginReceive( mByBuff, 0, mByBuff.Length, SocketFlags.None, recieveData, this );
			}
			catch( Exception ex )
			{
				Console.WriteLine( "Recieve callback setup failed! {0}", ex.Message );
			}
		}

		/// <summary>
		/// Data has been recieved so we shall put it in an array and
		/// return it.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns>Array of bytes containing the received data</returns>
		public byte [] GetRecievedData( IAsyncResult ar )
		{
			int nBytesRec = 0;
			try
			{
				nBytesRec = mSock.EndReceive( ar );
			}
			catch
			{}
			byte [] byReturn = new byte[nBytesRec];
			Array.Copy( mByBuff, byReturn, nBytesRec );
			
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
