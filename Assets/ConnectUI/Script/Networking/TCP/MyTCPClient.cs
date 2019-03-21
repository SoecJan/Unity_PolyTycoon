using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;

namespace tcpTestClient.src.networking
{
	/// <summary>
	/// An instance of this class can connect to a server via TCP.
	/// It has callbacks for connect, disconnect and data receiving events.
	/// Data is received as String. Therefore JSON-Format is supported.
	/// </summary>
	class MyTCPClient
	{
		// #### Attributes #### //

		/// <summary>
		/// Called after establishing a server connection
		/// String being the Server ipAdress
		/// int being the Server port
		/// </summary>
		public System.Action<String, int> onServerConnect;
		/// <summary>
		/// Called after losing a connection to the Server
		/// </summary>
		public System.Action<String> onServerDisconnect;
		/// <summary>
		/// Get's called after receiving data by the server. Data is of type String. Probably a JSON that needs to be parsed.
		/// </summary>
		public System.Action<String> onDataReceiveCallBack;

		/// <summary>
		/// The ipAdress to connect to
		/// </summary>
		private String ipAdress;
		/// <summary>
		/// The port to connect to
		/// </summary>
		private int port;

		/// <summary>
		/// TCPClient object that is used to connect to a server
		/// </summary>
		private TcpClient tcpClient;
		/// <summary>
		/// The NetworkStream connected to the TCPClient-Object
		/// </summary>
		private NetworkStream networkStream;

		/// <summary>
		/// The Reader to Read Data from the NetworkStream
		/// </summary>
		private StreamReader streamReader;
		/// <summary>
		/// The Writer to Write Data to the NetworkStream
		/// </summary>
		private StreamWriter streamWriter;

		/// <summary>
		/// The Thread that listens to data that was send by the server
		/// </summary>
		private Thread receiverThread;
		/// <summary>
		/// The time the receiverThread sleeps between listenings.
		/// </summary>
		private const int ReceiverThreadSleepTime = 30;

		/// <summary>
		/// The Thread that constantly tries to connect the client to a server.
		/// </summary>
		private Thread connectThread;
		/// <summary>
		/// The time the connectThread sleeps between connection attempts.
		/// </summary>
		private const int ConnectThreadSleepTime = 30;

		// #### Constructors #### //

		/// <summary>
		/// Constructs a client that automatically tries to connect on given ipAdress and port. 
		/// There should always only be one instance of this object.
		/// To stop a client -> call Dispose();
		/// <para></para>
		/// If you use this constructor you need to make sure that ipAdress and port are set before calling ConnectToServer();
		/// </summary>
		public MyTCPClient()
		{ }

		/// <summary>
		/// Constructs a client that automatically tries to connect on given ipAdress and port. 
		/// There should always only be one instance of this object.
		/// To start a client -> call Start();
		/// To stop a client -> call Dispose();
		/// </summary>
		/// <param name="ipAdress"></param>
		/// <param name="port"></param>
		public MyTCPClient(String ipAdress, int port) : this()
		{
			this.IpAdress = ipAdress;
			this.Port = port;
		}

		// #### Getter & Setter #### //

		public string IpAdress { get { return ipAdress; } set { ipAdress = value; }}
		public int Port { get { return port; } set { port = value; }}
		public TcpClient MyTcpClient { get { return tcpClient; } set { tcpClient = value; } }
		public NetworkStream NetworkStream { get { return networkStream; } private set { networkStream = value; } }
		public StreamReader StreamReader { get { return streamReader; } private set { streamReader = value; }}
		public StreamWriter StreamWriter { get { return streamWriter; } private set { streamWriter = value; }}
		public Thread ReceiverThread { get { return receiverThread; } private set { receiverThread = value; }}
		public Thread ConnectThread { get { return connectThread; } private set { connectThread = value; }}

		// #### Methods #### //

		/// <summary>
		/// The connection process to the server.
		/// Should be run by a different Thread.
		/// After successful connection the callback onServerConnect(ipAdress, port) is called.
		/// </summary>
		private bool ConnectToServer()
		{
			try
			{
				if (Port < IPEndPoint.MinPort || Port > IPEndPoint.MaxPort)
					return false;
				MyTcpClient = new TcpClient(ipAdress, Port); // try to connect
				NetworkStream = MyTcpClient.GetStream(); // as connect was successful -> GetNetworkStream

				StreamReader = new StreamReader(NetworkStream, System.Text.Encoding.UTF8); // Create StreamReader for reading NetworkData
				StreamWriter = new StreamWriter(NetworkStream, System.Text.Encoding.UTF8); // Create StreamWriter for writing NetworkData

				ReceiverThread = new Thread(ReceiveData); // Keep listening for incoming data on a different Thread
				ReceiverThread.Start();

				if (onServerConnect != null) { // notify observers of a successful connection
					onServerConnect(ipAdress, Port);
				} 
				connectThread = null;
				return true;
			}
			catch (SocketException)
			{
			}
			return false;
		}

		/// <summary>
		/// Needs to be called to start this client.
		/// Errors might be written to the console.
		/// </summary>
		/// <returns>true if ipAdress was correct and connection process starts.</returns>
		public bool Start()
		{
			if (this.IpAdress != null && this.Port != 0)
			{
				IPAddress outAddress;
				if (IPAddress.TryParse(this.IpAdress, out outAddress) || this.IpAdress.Equals("localhost")) // Verify IP-Adress being valid
				{
					return this.ConnectToServer(); // Connection Process can begin
				}
				else
				{
					Console.Error.WriteLine("Server IP-Adress: " + this.IpAdress + " is not a valid IP-Adress.");
				}
			}
			else
			{
				Console.Error.WriteLine("Server IP-Adress or Port are not set.");
			}
			return false; // Connection Process can't begin
		}

		/// <summary>
		/// Sends data to the connected server.
		/// If there is currently no server connected an error is written to the console.
		/// </summary>
		/// <param name="data"></param>
		public void SendData(String data)
		{
			if (StreamWriter != null) // might by null if a connection was not established yet
			{
				StreamWriter.Write(data);
				StreamWriter.Flush();
			}
			else
			{
				Console.Error.WriteLine(
					"Data can't be send. " +
					"Client might not be connected yet. " +
					"You need to call client.Start() and wait for a successful connection. " +
					"Subscribe and wait for the Action client.onServerConnect().");
			}
		}

		/// <summary>
		/// Should be run in a different Thread as this method synchroniously waits for Data from the server in an infinite loop.
		/// To read Data subscribe to client.OnDataReceiveCallback();
		/// </summary>
		private void ReceiveData()
		{
			StringBuilder stringBuilder = new StringBuilder(); // Use StringBuilder for better performance than String+=String;
			int jsonDelimiterIndex = 0; // if this index is 0 a new jsonObject is started
			char currentChar;
			while (true)
			{
				try
				{
					if (!this.StreamReader.EndOfStream) // Execute as we are not at the end of the stream (somehow needed to read more than one message)
					{
						while (this.StreamReader.Peek() != -1) // Read data while new data is available
						{
							currentChar = Convert.ToChar(StreamReader.Read());
							stringBuilder.Append(currentChar); // Read data from stream as string
							if (currentChar.Equals('{'))
							{
								jsonDelimiterIndex += 1;
							} 
							else if(currentChar.Equals('}'))
							{
								jsonDelimiterIndex -= 1;
							}
							if (jsonDelimiterIndex == 0 && stringBuilder.Length > 0)
							{
								if (onDataReceiveCallBack != null) {
									onDataReceiveCallBack(stringBuilder.ToString());// Notify observers
								} 
								stringBuilder = new StringBuilder(); // reset stringbuilder
							}
							else if (jsonDelimiterIndex < 0)
							{
								Console.Error.WriteLine("An Error has occurred. Received Data was not readable. Resetting.");
								jsonDelimiterIndex = 0;
								stringBuilder = new StringBuilder(); // reset stringbuilder
							}
						}
						Thread.Sleep(ReceiverThreadSleepTime); // sleep for 30 millis. Doesn't need to run as fast as possible
					}
				}
				catch (ObjectDisposedException) // StreamReader was Disposed
				{
					return;
				}
				catch (NullReferenceException) // StreamReader not yet set
				{
					return;
				}
				catch (IOException) // Server disconnected
				{
					if (onServerDisconnect != null) {
						onServerDisconnect ("Connection Lost");
					} // Callback for observers
					Dispose(); // Dispose all allocated Resources
					return;
				}
			}
		}

		/// <summary>
		/// Disconnects this client from the server. Disposes allocated Objects.
		/// </summary>
		public void Dispose()
		{
			if (StreamWriter != null)
				StreamWriter.Close();
			if (StreamReader != null)
				StreamReader.Close();
			if (NetworkStream != null)
				NetworkStream.Close();
			if (MyTcpClient != null)
				MyTcpClient.Close();
		}
	}

	interface ITCPClientController
	{
		void SendDataTCP(String data);
		
		void OnDataReceive(String data);

		void OnServerConnectionEstablished(String ipAdress, int port);

		void OnServerConnectionLost(String reason);

		void Dispose();
	}
}
