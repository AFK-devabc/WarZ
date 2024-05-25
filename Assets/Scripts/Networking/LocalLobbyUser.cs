using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalLobbyUser
{
	public event Action<LocalLobbyUser> changed;

	public LocalLobbyUser()
	{
		m_UserData = new UserData(isHost: false, displayName: null, id: null, characterIndex: null, weaponIndex:null, isReady:"false");
	}

	public struct UserData
	{
		public bool IsHost { get; set; }
		public string DisplayName { get; set; }
		public string ID { get; set; }
		public string CharacterIndex { get; set; }
		public string WeaponIndex { get; set; }
		public string IsReady { get; set; }


		public UserData(bool isHost, string displayName, string id, string characterIndex, string weaponIndex, string isReady)
		{
			IsHost = isHost;
			DisplayName = displayName;
			ID = id;
			CharacterIndex = characterIndex;
			WeaponIndex = weaponIndex;
			IsReady = isReady;
		}
	}

	UserData m_UserData;

	public void ResetState()
	{
		m_UserData = new UserData(false, m_UserData.DisplayName, m_UserData.ID, m_UserData.CharacterIndex, m_UserData.WeaponIndex, "false");
	}

	/// <summary>
	/// Used for limiting costly OnChanged actions to just the members which actually changed.
	/// </summary>
	[Flags]
	public enum UserMembers
	{
		IsHost = 1,
		DisplayName = 2,
		ID = 4,
		CharacterIndex = 8,
		WeaponIndex = 16,
	}

	UserMembers m_LastChanged;

	public bool IsHost
	{
		get { return m_UserData.IsHost; }
		set
		{
			if (m_UserData.IsHost != value)
			{
				m_UserData.IsHost = value;
				m_LastChanged = UserMembers.IsHost;
				OnChanged();
			}
		}
	}

	public string DisplayName
	{
		get => m_UserData.DisplayName;
		set
		{
			if (m_UserData.DisplayName != value)
			{
				m_UserData.DisplayName = value;
				m_LastChanged = UserMembers.DisplayName;
				OnChanged();
			}
		}
	}

	public string ID
	{
		get => m_UserData.ID;
		set
		{
			if (m_UserData.ID != value)
			{
				m_UserData.ID = value;
				m_LastChanged = UserMembers.ID;
				OnChanged();
			}
		}
	}

	public string CharacterIndex
	{
		get => m_UserData.CharacterIndex;
		set
		{
			if (m_UserData.CharacterIndex != value)
			{
				m_UserData.CharacterIndex = value;
				m_LastChanged = UserMembers.CharacterIndex;
				OnChanged();
			}
		}
	}


	public string WeaponIndex
	{
		get => m_UserData.WeaponIndex;
		set
		{
			if (m_UserData.WeaponIndex != value)
			{
				m_UserData.WeaponIndex = value;
				m_LastChanged = UserMembers.WeaponIndex;
				OnChanged();
			}
		}
	}

	public string IsReady
	{
		get => m_UserData.IsReady;
		set
		{
			if (m_UserData.IsReady != value)
			{
				m_UserData.IsReady = value;
				m_LastChanged = UserMembers.WeaponIndex;
				OnChanged();
			}
		}
	}


	public void CopyDataFrom(LocalLobbyUser lobby)
	{
		var data = lobby.m_UserData;
		int lastChanged = // Set flags just for the members that will be changed.
			(m_UserData.IsHost == data.IsHost ? 0 : (int)UserMembers.IsHost) |
			(m_UserData.DisplayName == data.DisplayName ? 0 : (int)UserMembers.DisplayName) |
			(m_UserData.IsReady == data.IsReady ? 0 : (int)UserMembers.ID)|
			(m_UserData.ID == data.ID ? 0 : (int)UserMembers.ID);

		if (lastChanged == 0) // Ensure something actually changed.
		{
			return;
		}

		m_UserData = data;
		m_LastChanged = (UserMembers)lastChanged;

		OnChanged();
	}

	void OnChanged()
	{
		changed?.Invoke(this);
	}

	public Dictionary<string, PlayerDataObject> GetDataForUnityServices() =>
		new Dictionary<string, PlayerDataObject>()
		{
				{PlayerDataDefined.m_Name, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, DisplayName)},
				{PlayerDataDefined.m_CharacterIndex, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CharacterIndex)},
				{PlayerDataDefined.m_Weapon, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, WeaponIndex)},
				{PlayerDataDefined.m_IsReady, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, IsReady)},
		};
}

static class PlayerDataDefined
{
	public const string m_Name = "Player Name";
	public const string m_CharacterIndex = "Character Index";
	public const string m_Weapon = "Weapon";
	public const string m_IsReady = "IsReady";

}
