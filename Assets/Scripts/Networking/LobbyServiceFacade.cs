using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System;
using UnityEngine.Events;


/// <summary>
/// An abstraction layer between the direct calls into the Lobby API and the outcomes you actually want.
/// </summary>

public class LobbyServiceFacade 
{
	public LocalLobby m_LocalLobby { private set; get; }
	public LocalLobbyUser m_LocalUser { private set; get; }
	private UpdateRunner m_UpdateRunner;
	const float k_HeartbeatPeriod = 8; // The heartbeat must be rate-limited to 5 calls per 30 seconds. We'll aim for longer in case periods don't align.
	float m_HeartbeatTime = 0;

	LobbyAPI m_LobbyApiInterface;

	RateLimitCooldown m_RateLimitQuery;
	RateLimitCooldown m_RateLimitJoin;
	RateLimitCooldown m_RateLimitHost;

	public Lobby CurrentUnityLobby { get; private set; }

	ILobbyEvents m_LobbyEvents;

	bool m_IsTracking = false;

	LobbyEventConnectionState m_LobbyEventConnectionState = LobbyEventConnectionState.Unknown;

	public LobbyServiceFacade(LocalLobby i_locallobby, LocalLobbyUser i_localLobbyUser, UpdateRunner i_updateRunner)
	{
		m_RateLimitQuery = new RateLimitCooldown(1f);
		m_RateLimitJoin = new RateLimitCooldown(3f);
		m_RateLimitHost = new RateLimitCooldown(3f);

		m_LocalLobby = i_locallobby;
		m_LocalUser = i_localLobbyUser;
		m_UpdateRunner = i_updateRunner;

		m_LobbyApiInterface = new LobbyAPI();
	}

	public void Dispose()
	{
		EndTracking();
	}

	public void SetRemoteLobby(Lobby lobby)
	{
		CurrentUnityLobby = lobby;
		m_LocalLobby.ApplyRemoteData(lobby);
	}

	void DoLobbyHeartbeat(float dt)
	{
		m_HeartbeatTime += dt;
		if (m_HeartbeatTime > k_HeartbeatPeriod)
		{
			m_HeartbeatTime -= k_HeartbeatPeriod;
			try
			{
				m_LobbyApiInterface.HeartbeatLobby(CurrentUnityLobby.Id);
			}
			catch (LobbyServiceException e)
			{
				// If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
				if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
				{
					PublishError(e);
				}
			}
		}
	}

	/// <summary>
	/// Initiates tracking of joined lobby's events. The host also starts sending heartbeat pings here.
	/// </summary>
	public void BeginTracking()
	{
		if (!m_IsTracking)
		{
			m_IsTracking = true;
			SubscribeToJoinedLobbyAsync();

			// Only the host sends heartbeat pings to the service to keep the lobby alive
			if (m_LocalUser.IsHost)
			{
				m_HeartbeatTime = 0;
				m_UpdateRunner.Subscribe(DoLobbyHeartbeat, 1.5f);
			}
		}
	}

	/// <summary>
	/// Ends tracking of joined lobby's events and leaves or deletes the lobby. The host also stops sending heartbeat pings here.
	/// </summary>
	public void EndTracking()
	{
		Utils.OnLeaveLobbySuccessEvent?.Invoke();

		if (m_IsTracking)
		{
			m_IsTracking = false;
			UnsubscribeToJoinedLobbyAsync();

			// Only the host sends heartbeat pings to the service to keep the lobby alive
			if (m_LocalUser.IsHost)
			{
				m_UpdateRunner.Unsubscribe(DoLobbyHeartbeat);
			}
		}

		if (CurrentUnityLobby != null)
		{
			if (m_LocalUser.IsHost)
			{
				DeleteLobbyAsync();
			}
			else
			{
				LeaveLobbyAsync();
			}
		}
	}

	/// <summary>
	/// Attempt to create a new lobby and then join it.
	/// </summary>
	public async Task<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxPlayers, bool isPrivate)
	{
		if (!m_RateLimitHost.CanCall)
		{
			Debug.Log("Create Lobby hit the rate limit.");
			return (false, null);
		}

		try
		{
			var lobby = await m_LobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId, lobbyName, maxPlayers, isPrivate, m_LocalUser.GetDataForUnityServices(), m_LocalLobby.GetDataForUnityServices()) ;
			Utils.OnJoinLobbySuccessEvent?.Invoke();
			return (true, lobby);
		}
		catch (LobbyServiceException e)
		{
			if (e.Reason == LobbyExceptionReason.RateLimited)
			{
				m_RateLimitHost.PutOnCooldown();
			}
			else
			{
				PublishError(e);
			}
		}

		return (false, null);
	}

	/// <summary>
	/// Attempt to join an existing lobby. Will try to join via code, if code is null - will try to join via ID.
	/// </summary>
	public async Task<(bool Success, Lobby Lobby)> TryJoinLobbyAsync(string lobbyId, string lobbyCode)
	{
		if (!m_RateLimitJoin.CanCall ||
			(lobbyId == null && lobbyCode == null))
		{
			Debug.LogWarning("Join Lobby hit the rate limit.");
			return (false, null);
		}

		try
		{
			if (!string.IsNullOrEmpty(lobbyCode))
			{
				JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
				options.Player =new Player(id:m_LocalUser.ID, data: m_LocalUser.GetDataForUnityServices());
				var lobby = await m_LobbyApiInterface.JoinLobbyByCode(lobbyCode, options);
				Utils.OnJoinLobbySuccessEvent?.Invoke();
				return (true, lobby);
			}
		}
		catch (LobbyServiceException e)
		{
			if (e.Reason == LobbyExceptionReason.RateLimited)
			{
				m_RateLimitJoin.PutOnCooldown();
			}
			else
			{
				Debug.Log(e);
			}
		}

		return (false, null);
	}

	void ResetLobby()
	{
		CurrentUnityLobby = null;



		if (m_LocalUser != null)
		{
			m_LocalUser.ResetState();
		}
		if (m_LocalLobby != null)
		{
			m_LocalLobby.Reset(m_LocalUser);
		}

		// no need to disconnect Netcode, it should already be handled by Netcode's callback to disconnect
	}

	void OnLobbyChanges(ILobbyChanges changes)
	{
		if (changes.LobbyDeleted)
		{
			Debug.Log("Lobby deleted");
			ResetLobby();
			EndTracking();
		}
		else
		{
			Debug.Log("Lobby updated");
			changes.ApplyToLobby(CurrentUnityLobby);
			m_LocalLobby.ApplyRemoteData(CurrentUnityLobby);

			// as client, check if host is still in lobby
			if (!m_LocalUser.IsHost)
			{
				foreach (var lobbyUser in m_LocalLobby.LobbyUsers)
				{
					if (lobbyUser.Value.IsHost)
					{
						return;
					}
				}
				EndTracking();
				// no need to disconnect Netcode, it should already be handled by Netcode's callback to disconnect
			}
		}
	}

	void OnKickedFromLobby()
	{
		Debug.Log("Kicked from Lobby");
		ResetLobby();
		EndTracking();
	}

	void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState lobbyEventConnectionState)
	{
		m_LobbyEventConnectionState = lobbyEventConnectionState;
		Debug.Log($"LobbyEventConnectionState changed to {lobbyEventConnectionState}");
	}

	async void SubscribeToJoinedLobbyAsync()
	{
		var lobbyEventCallbacks = new LobbyEventCallbacks();
		lobbyEventCallbacks.LobbyChanged += OnLobbyChanges;
		lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
		lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
		// The LobbyEventCallbacks object created here will now be managed by the Lobby SDK. The callbacks will be
		// unsubscribed from when we call UnsubscribeAsync on the ILobbyEvents object we receive and store here.
		m_LobbyEvents = await m_LobbyApiInterface.SubscribeToLobby(m_LocalLobby.LobbyID, lobbyEventCallbacks);
	}

	async void UnsubscribeToJoinedLobbyAsync()
	{
		if (m_LobbyEvents != null && m_LobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed)
		{
			await m_LobbyEvents.UnsubscribeAsync();
		}
	}

	public async Task<Lobby> ReconnectToLobbyAsync()
	{
		try
		{
			return await m_LobbyApiInterface.ReconnectToLobby(m_LocalLobby.LobbyID);
		}
		catch (LobbyServiceException e)
		{
			// If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
			if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
			{
				PublishError(e);
			}
		}

		return null;
	}

	/// <summary>
	/// Attempt to leave a lobby
	/// </summary>
	async void LeaveLobbyAsync()
	{
		string uasId = AuthenticationService.Instance.PlayerId;



		try
		{
			await m_LobbyApiInterface.RemovePlayerFromLobby(uasId, m_LocalLobby.LobbyID);
		}
		catch (LobbyServiceException e)
		{
			// If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
			if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
			{
				PublishError(e);
			}
		}
		finally
		{
			ResetLobby();

			Debug.Log("Leaved lobby");
		}

	}

	public async void RemovePlayerFromLobbyAsync(string uasId)
	{
		if (m_LocalUser.IsHost)
		{
			try
			{
				await m_LobbyApiInterface.RemovePlayerFromLobby(uasId, m_LocalLobby.LobbyID);
			}
			catch (LobbyServiceException e)
			{
				Debug.Log(e);
			}
		}
		else
		{
			Debug.LogError("Only the host can remove other players from the lobby.");
		}
	}

	async void DeleteLobbyAsync()
	{
		if (m_LocalUser.IsHost)
		{
			try
			{
				await m_LobbyApiInterface.DeleteLobby(m_LocalLobby.LobbyID);
			}
			catch (LobbyServiceException e)
			{
				PublishError(e);
			}
			finally
			{
				ResetLobby();
				Debug.Log("Leaved lobby");
			}
		}
		else
		{
			Debug.LogError("Only the host can delete a lobby.");
		}
	}

	/// <summary>
	/// Attempt to push a set of key-value pairs associated with the local player which will overwrite any existing
	/// data for these keys. Lobby can be provided info about Relay (or any other remote allocation) so it can add
	/// automatic disconnect handling.
	/// </summary>
	public async Task UpdatePlayerDataAsync(string allocationId, string connectionInfo)
	{
		if (!m_RateLimitQuery.CanCall)
		{
			return;
		}

		try
		{
			var result = await m_LobbyApiInterface.UpdatePlayer(CurrentUnityLobby.Id, m_LocalUser.ID, m_LocalUser.GetDataForUnityServices(), allocationId, connectionInfo);

			if (result != null)
			{
				SetRemoteLobby(result);
			}
		}
		catch (LobbyServiceException e)
		{
			if (e.Reason == LobbyExceptionReason.RateLimited)
			{
				m_RateLimitQuery.PutOnCooldown();
			}
			else if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost) // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
			{
				PublishError(e);
			}
		}
	}

	/// <summary>
	/// Attempt to update the set of key-value pairs associated with a given lobby and unlocks or unlocl it so clients can see it.
	/// </summary>
	public async Task UpdateLobbyDataAndChangeLockStatusAsync(bool shouldLock = false)
	{
		if (!m_RateLimitQuery.CanCall)
		{
			return;
		}

		var localData = m_LocalLobby.GetDataForUnityServices();

		var dataCurr = CurrentUnityLobby.Data;
		if (dataCurr == null)
		{
			dataCurr = new Dictionary<string, DataObject>();
		}

		foreach (var dataNew in localData)
		{
			if (dataCurr.ContainsKey(dataNew.Key))
			{
				dataCurr[dataNew.Key] = dataNew.Value;
			}
			else
			{
				dataCurr.Add(dataNew.Key, dataNew.Value);
			}
		}

		try
		{
			var result = await m_LobbyApiInterface.UpdateLobby(CurrentUnityLobby.Id, dataCurr, i_shouldLock: shouldLock);

			if (result != null)
			{
				CurrentUnityLobby = result;
			}
		}
		catch (LobbyServiceException e)
		{
			if (e.Reason == LobbyExceptionReason.RateLimited)
			{
				m_RateLimitQuery.PutOnCooldown();
			}
			else
			{
				PublishError(e);
			}
		}
	}

	void PublishError(LobbyServiceException e)
	{
		var reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})"; // Lobby error type, then HTTP error type.
		//m_UnityServiceErrorMessagePub.Publish(new UnityServiceErrorMessage("Lobby Error", reason, UnityServiceErrorMessage.Service.Lobby, e));
	}
}

