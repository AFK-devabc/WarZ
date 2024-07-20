using System;
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

	public static List<ObjectInGame> ObjectsInGames = new List<ObjectInGame>();

	public static Action<ObjectInGame> NewObjectAddedEvent;
	public static Action<ObjectInGame> ObjectRemovedEvent;


	public static void AddNewObject(Transform transform, ObjectType type)
	{
		ObjectInGame newObject = new ObjectInGame(transform, type);
		ObjectsInGames.Add(newObject);

		NewObjectAddedEvent?.Invoke(newObject);
	}

	public static void RemoveObject(Transform transform)
	{
		foreach (var obj in ObjectsInGames)
		{
			if (obj.m_transform == transform)
			{
				ObjectsInGames.Remove(obj);
				ObjectRemovedEvent.Invoke(obj);
				return;
			}
		}
	}

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

	public static Transform camera;
}

public class ObjectInGame
{
	public Transform m_transform;
	public ObjectType m_type;

	public ObjectInGame(Transform transform, ObjectType type)
	{
		m_transform = transform;
		m_type = type;
	}
}

public enum ObjectType
{
	LocalPlayer,
	Ally,
	NormalEnemy,
	Boss,
	Objective
}