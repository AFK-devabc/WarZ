using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using Unity.Netcode.Transports.UTP;


public class ApplicationController : MonoBehaviour
{
	public LocalLobby m_LocalLobby { private set; get; }
	public LocalLobbyUser m_LocalLobbyUser { private set; get; }

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
	}

	private Dictionary<string, string> GetCommandlineArgs()
	{
		Dictionary<string, string> argDictionary = new Dictionary<string, string>();

		var args = System.Environment.GetCommandLineArgs();

		for (int i = 0; i < args.Length; ++i)
		{
			var arg = args[i].ToLower();
			if (arg.StartsWith("-"))
			{
				var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
				value = (value?.StartsWith("-") ?? false) ? null : value;

				argDictionary.Add(arg, value);
			}
		}
		return argDictionary;
	}

}

