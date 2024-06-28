using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Utils
{
	public static UnityAction OnJoinLobbySuccessEvent;
	public static UnityAction OnLeaveLobbySuccessEvent;
	public static UnityAction OnEnterCustomCharacterEvent;
	public static UnityAction OnLeaveCustomCharacterEvent;

	public static bool IsPointerOverUIObject()
	{
		if (EventSystem.current == null) return true;

		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public static NetworkPlayer m_localNetworkPlayer;

	public static bool isClientCharacterSetupDone = false;

	public static UnityAction<NetworkPlayer> OnClientCharacterSetupDone;

	public static Transform RecursiveFindChild(Transform parent, string childName)
	{
		foreach (Transform child in parent)
		{
			if (child.name == childName)
			{
				return child;
			}
			else
			{
				Transform found = RecursiveFindChild(child, childName);
				if (found != null)
				{
					return found;
				}
			}
		}
		return null;
	}

}
