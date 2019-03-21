using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MyUdpClient
{
	public static String SERVER_SEARCH_MESSAGE = "{\"type\":\"ServerSearchBroadcast\"}";
	public System.Action<String> OnUdpMessageReceive;
	private int port = 11000;
	private UdpClient client;
	private IPEndPoint ip;

	public MyUdpClient()
	{
		Start();
	}

	public void Start() 
	{
		ip = new IPEndPoint(IPAddress.Broadcast, port);
		client = new UdpClient();
		new Thread(Receive).Start();
	}

	private void Receive()
	{
		try
		{
			client.BeginReceive(new AsyncCallback(OnReceive), null);
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public void Send(String message)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		client.Send(bytes, bytes.Length, ip);
	}

	public void Close()
	{
		if (client != null)
		client.Close();
		client = null;
	}

	//CallBack
	private void OnReceive(IAsyncResult res)
	{
		if (client != null)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 11000);
			byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

			//Process codes
			OnUdpMessageReceive(Encoding.UTF8.GetString(received));
			client.BeginReceive(new AsyncCallback(OnReceive), null);
		}
	}
}
