using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class TestNetwork : MonoBehaviour
{
	public void StartHost()
	{
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
									"127.0.0.1",  // The IP address is a string
									(ushort)12345, // The port number is an unsigned short
									"0.0.0.0" // The server listen address is a string.
									);

		NetworkManager.Singleton.StartHost();
	}
	public void StartClient ()
	{
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
							"127.0.0.1",  // The IP address is a string
							(ushort)12345, // The port number is an unsigned short
							"0.0.0.0" // The server listen address is a string.
							);

		NetworkManager.Singleton.StartClient();
	}
}
