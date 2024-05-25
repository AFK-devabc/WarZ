using System.Threading.Tasks;
using Unity.Services.Authentication;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;


public class LobbyAPI
{
	public LobbyAPI() { }
	
	public async Task<Lobby> CreateLobby(string i_requesterUasId, string i_lobbyName, int i_maxPlayers, bool i_isPrivate, Dictionary<string, PlayerDataObject> i_hostUserData, Dictionary<string, DataObject> i_lobbyData)
	{
		CreateLobbyOptions createOptions = new CreateLobbyOptions
		{
			IsPrivate = i_isPrivate,
			IsLocked = true, // locking the lobby at creation to prevent other players from joining before it is ready
			Player = new Player(id: i_requesterUasId, data: i_hostUserData),
			Data = i_lobbyData
		};

		return await LobbyService.Instance.CreateLobbyAsync(i_lobbyName, i_maxPlayers, createOptions);
	}

	public async Task<Lobby> UpdateLobby(string i_lobbyId, Dictionary<string, DataObject> i_data, bool i_shouldLock)
	{
		UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = i_data, IsLocked = i_shouldLock };
		return await LobbyService.Instance.UpdateLobbyAsync(i_lobbyId, updateOptions);
	}

	public async Task<Lobby> UpdatePlayer(string i_lobbyId, string i_playerId, Dictionary<string, PlayerDataObject> i_data, string i_allocationId, string i_connectionInfo)
	{
		UpdatePlayerOptions updateOptions = new UpdatePlayerOptions
		{
			Data = i_data
			//AllocationId = i_allocationId,
			//ConnectionInfo = i_connectionInfo
		};
		return await LobbyService.Instance.UpdatePlayerAsync(i_lobbyId, i_playerId, updateOptions);
	}

	public async void HeartbeatLobby(string i_lobbyID)
	{
		await LobbyService.Instance.SendHeartbeatPingAsync(i_lobbyID);
	}

	public async Task<Lobby> JoinLobbyByCode(string i_lobbyCode, JoinLobbyByCodeOptions i_option)
	{
		return await LobbyService.Instance.JoinLobbyByCodeAsync(i_lobbyCode, i_option);
	}

	public async Task<ILobbyEvents> SubscribeToLobby(string i_lobbyCode, LobbyEventCallbacks i_eventCallbacks)
	{
		return await LobbyService.Instance.SubscribeToLobbyEventsAsync(i_lobbyCode, i_eventCallbacks);
	}

	public async Task RemovePlayerFromLobby(string i_requesterUasId, string i_lobbyId)
	{
		try
		{
			await LobbyService.Instance.RemovePlayerAsync(i_lobbyId, i_requesterUasId);
		}
		catch (LobbyServiceException e)
			when (e is { Reason: LobbyExceptionReason.PlayerNotFound })
		{
			// If Player is not found, they have already left the lobby or have been kicked out. No need to throw here
		}
	}

	public async Task DeleteLobby(string i_lobbyId)
	{
		await LobbyService.Instance.DeleteLobbyAsync(i_lobbyId);
	}

	public async Task<Lobby> ReconnectToLobby(string i_lobbyId)
	{
		return await LobbyService.Instance.ReconnectToLobbyAsync(i_lobbyId);
	}


	public void LogLobbyData(Lobby i_Lobby)
	{
		foreach (Player player in i_Lobby.Players)
		{
			Debug.Log(player.Data[PlayerDataDefined.m_Name]);
		}
	}
}
