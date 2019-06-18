using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;

public class SuperCollider : MonoBehaviour
{
	public InputField ipAddress;
	public InputField port;

	private OSCClient client = null;

	private void Awake()
	{
		Connect();
	}

	public void Connect()
	{
		Connect(ipAddress.text, int.Parse(port.text));
	}

	public void Connect(string address, int port)
	{
		try
		{
			client = new OSCClient(IPAddress.Parse(address), port);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e);
			client = null;
		}
	}

	private void Send(OSCPacket packet)
	{
		try
		{
			client.Send(packet);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e);
		}
	}

	public void SendBounce(int layerCount)
	{
		var message = new OSCMessage("/bounce");
		message.Append(layerCount);
		Send(message);
	}
}
