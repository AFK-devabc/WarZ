using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[Serializable]
public sealed class LocalLobby
{
	public event Action<LocalLobby> changed;

	///// <summary>
	///// Create a list of new LocalLobbies from the result of a lobby list query.
	///// </summary>
	//public static List<LocalLobby> CreateLocalLobbies(QueryResponse response)
	//{
	//	var retLst = new List<LocalLobby>();
	//	foreach (var lobby in response.Results)
	//	{
	//		retLst.Add(Create(lobby));
	//	}
	//	return retLst;
	//}

	//public static LocalLobby Create(Lobby lobby)
	//{
	//	var data = new LocalLobby();
	//	data.ApplyRemoteData(lobby);
	//	return data;
	//}

	Dictionary<string, LocalLobbyUser> m_LobbyUsers = new Dictionary<string, LocalLobbyUser>();
	public Dictionary<string, LocalLobbyUser> LobbyUsers => m_LobbyUsers;

	public struct LobbyData
	{
		public string LobbyID { get; set; }
		public string LobbyCode { get; set; }
		public string LobbyName { get; set; }
		public bool Private { get; set; }
		public int MaxPlayerCount { get; set; }
		public string IsStarted { get; set; }
		public string ServerIP { get; set; }
		public string ServerPort { get; set; }
		public string ServerListenAddress { get; set; }

		public LobbyData(LobbyData existing)
		{
			LobbyID = existing.LobbyID;
			LobbyCode = existing.LobbyCode;
			LobbyName = existing.LobbyName;
			Private = existing.Private;
			MaxPlayerCount = existing.MaxPlayerCount;
			IsStarted = existing.IsStarted;
			ServerIP = existing.ServerIP;
			ServerPort = existing.ServerPort;
			ServerListenAddress = existing.ServerListenAddress;
		}

		public LobbyData(string lobbyCode)
		{
			LobbyID = null;
			LobbyCode = lobbyCode;
			LobbyName = null;
			Private = false;
			MaxPlayerCount = -1;
			IsStarted = "False";
			ServerIP = null;
			ServerPort = null;
			ServerListenAddress = null;
		}
	}

	LobbyData m_Data;
	public LobbyData Data => new LobbyData(m_Data);

	public void AddUser(LocalLobbyUser user)
	{
		if (!m_LobbyUsers.ContainsKey(user.ID))
		{
			DoAddUser(user);
			OnChanged();
		}
	}

	void DoAddUser(LocalLobbyUser user)
	{
		m_LobbyUsers.Add(user.ID, user);
		user.changed += OnChangedUser;
	}

	public void RemoveUser(LocalLobbyUser user)
	{
		DoRemoveUser(user);
		OnChanged();
	}

	void DoRemoveUser(LocalLobbyUser user)
	{
		if (!m_LobbyUsers.ContainsKey(user.ID))
		{
			Debug.LogWarning($"Player {user.DisplayName}({user.ID}) does not exist in lobby: {LobbyID}");
			return;
		}

		m_LobbyUsers.Remove(user.ID);
		user.changed -= OnChangedUser;
	}

	void OnChangedUser(LocalLobbyUser user)
	{
		OnChanged();
	}

	void OnChanged()
	{
		changed?.Invoke(this);
	}

	public string LobbyID
	{
		get => m_Data.LobbyID;
		set
		{
			m_Data.LobbyID = value;
			OnChanged();
		}
	}

	public string LobbyCode
	{
		get => m_Data.LobbyCode;
		set
		{
			m_Data.LobbyCode = value;
			OnChanged();
		}
	}

	public string LobbyName
	{
		get => m_Data.LobbyName;
		set
		{
			m_Data.LobbyName = value;
			OnChanged();
		}
	}

	public bool Private
	{
		get => m_Data.Private;
		set
		{
			m_Data.Private = value;
			OnChanged();
		}
	}

	public int PlayerCount => m_LobbyUsers.Count;

	public int MaxPlayerCount
	{
		get => m_Data.MaxPlayerCount;
		set
		{
			m_Data.MaxPlayerCount = value;
			OnChanged();
		}
	}

	public string IsStarted
	{
		get => m_Data.IsStarted;
		set
		{
			m_Data.IsStarted = value;
			OnChanged();
		}
	}

	public string ServerIP
	{
		get => m_Data.ServerIP;
		set
		{
			m_Data.ServerIP = value;
			OnChanged();
		}
	}
	public string ServerPort
	{
		get => m_Data.ServerPort;
		set
		{
			m_Data.ServerPort = value;
			OnChanged();
		}
	}
	public string ServerListenAddress
	{
		get => m_Data.ServerListenAddress;
		set
		{
			m_Data.ServerListenAddress = value;
			OnChanged();
		}
	}


	public void CopyDataFrom(LobbyData data, Dictionary<string, LocalLobbyUser> currUsers)
	{
		m_Data = data;

		if (currUsers == null)
		{
			m_LobbyUsers = new Dictionary<string, LocalLobbyUser>();
		}
		else
		{
			List<LocalLobbyUser> toRemove = new List<LocalLobbyUser>();
			foreach (var oldUser in m_LobbyUsers)
			{
				if (currUsers.ContainsKey(oldUser.Key))
				{
					oldUser.Value.CopyDataFrom(currUsers[oldUser.Key]);
				}
				else
				{
					toRemove.Add(oldUser.Value);
				}
			}

			foreach (var remove in toRemove)
			{
				DoRemoveUser(remove);
			}

			foreach (var currUser in currUsers)
			{
				if (!m_LobbyUsers.ContainsKey(currUser.Key))
				{
					DoAddUser(currUser.Value);
				}
			}
		}

		OnChanged();
	}

	public Dictionary<string, DataObject> GetDataForUnityServices() =>
		new Dictionary<string, DataObject>()
		{
				{LobbyDataDefined.m_IsStarted, new DataObject(DataObject.VisibilityOptions.Public,  IsStarted)},
				{LobbyDataDefined.m_ServerIP, new DataObject(DataObject.VisibilityOptions.Public,  ServerIP)},
				{LobbyDataDefined.m_ServerPort, new DataObject(DataObject.VisibilityOptions.Public,  ServerPort)},
				{LobbyDataDefined.m_ServerListenAddress, new DataObject(DataObject.VisibilityOptions.Public,  ServerListenAddress)},
		};

	public void ApplyRemoteData(Lobby lobby)
	{
		var info = new LobbyData(); // Technically, this is largely redundant after the first assignment, but it won't do any harm to assign it again.
		info.LobbyID = lobby.Id;
		info.LobbyCode = lobby.LobbyCode;
		info.Private = lobby.IsPrivate;
		info.LobbyName = lobby.Name;
		info.MaxPlayerCount = lobby.MaxPlayers;

		if (lobby.Data != null)
		{
			info.ServerIP = lobby.Data.ContainsKey(LobbyDataDefined.m_ServerIP) ? lobby.Data[LobbyDataDefined.m_ServerIP].Value : null;
			info.ServerPort = lobby.Data.ContainsKey(LobbyDataDefined.m_ServerPort) ? lobby.Data[LobbyDataDefined.m_ServerPort].Value : null;
			info.ServerListenAddress = lobby.Data.ContainsKey(LobbyDataDefined.m_ServerListenAddress) ? lobby.Data[LobbyDataDefined.m_ServerListenAddress].Value : null; 
			info.IsStarted = lobby.Data.ContainsKey(LobbyDataDefined.m_IsStarted) ? lobby.Data[LobbyDataDefined.m_IsStarted].Value : null;

		}

		var lobbyUsers = new Dictionary<string, LocalLobbyUser>();
		foreach (var player in lobby.Players)
		{
			if (player.Data != null)
			{
				if (LobbyUsers.ContainsKey(player.Id))
				{
					LobbyUsers[player.Id].IsReady = player.Data[PlayerDataDefined.m_IsReady].Value;
					lobbyUsers.Add(player.Id, LobbyUsers[player.Id]);
					continue;
				}
			}

			// If the player isn't connected to Relay, get the most recent data that the lobby knows.
			// (If we haven't seen this player yet, a new local representation of the player will have already been added by the LocalLobby.)
			var incomingData = new LocalLobbyUser
			{
				IsHost = lobby.HostId.Equals(player.Id),
				DisplayName = player.Data[PlayerDataDefined.m_Name].Value,
				ID = player.Id,
				CharacterIndex = player.Data[PlayerDataDefined.m_CharacterIndex].Value,
				WeaponIndex = player.Data[PlayerDataDefined.m_Weapon].Value,
			};

			lobbyUsers.Add(incomingData.ID, incomingData);
		}

		CopyDataFrom(info, lobbyUsers);
	}

	public void Reset(LocalLobbyUser localUser)
	{
		CopyDataFrom(new LobbyData(), new Dictionary<string, LocalLobbyUser>());
		AddUser(localUser);
	}
}

static class LobbyDataDefined
{
	public const string m_Map = "Map name";
	public const string m_IsStarted = "Have started";
	public const string m_ServerIP = "Server IP";
	public const string m_ServerPort = "Server Port";
	public const string m_ServerListenAddress= "Server Listen Address";
}