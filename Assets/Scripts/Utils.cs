using System.Collections;
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
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
