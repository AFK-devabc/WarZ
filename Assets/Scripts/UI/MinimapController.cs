using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
	[SerializeField] private RectTransform miniMap;
	[SerializeField] private RectTransform iconContainers;
	private Transform toFocus;

	private Dictionary<ObjectInGame, MinimapObject> minimapObjects;

	[SerializeField] public float testScaleRatio = 1.5f;
	public static float scaleRatio;

	[SerializeField] private MinimapObject localPlayer;
	[SerializeField] private MinimapObject ally;
	[SerializeField] private MinimapObject enemy;
	[SerializeField] private MinimapObject boss;


	public void Initialize(float i_scaleRatio = 6.0f)
	{
		scaleRatio = i_scaleRatio;
		minimapObjects = new Dictionary<ObjectInGame, MinimapObject>();
		minimapObjects.Clear();
		foreach (var i in Utils.ObjectsInGames)
		{
			OnNewObjectAdded(i);
		}

		Utils.NewObjectAddedEvent += OnNewObjectAdded;
		Utils.ObjectRemovedEvent += RemoveObject;

		
	}

	private void OnDisable()
	{
		Utils.NewObjectAddedEvent -= OnNewObjectAdded;
		Utils.ObjectRemovedEvent -= RemoveObject;
	}

	public void SetToFocus(Transform i_toFocus)
	{
		toFocus = i_toFocus;
	}

	private void FixedUpdate()
	{
		scaleRatio = testScaleRatio;
		if (toFocus != null)
		{
			miniMap.localPosition = -TranslateToMapPosition(toFocus.position);
		}
	}

	public void OnNewObjectAdded(ObjectInGame objectInGame)
	{
		if (minimapObjects.ContainsKey(objectInGame))
			return;
		MinimapObject newminimapObject;

		switch (objectInGame.m_type)
		{
			case ObjectType.LocalPlayer:
				newminimapObject = Instantiate(localPlayer, iconContainers);
				break;
			case ObjectType.Ally:
				newminimapObject = Instantiate(ally, iconContainers);
				break;

			case ObjectType.NormalEnemy:
				newminimapObject = Instantiate(enemy, iconContainers);
				break;
			case ObjectType.Boss:
				newminimapObject = Instantiate(boss, iconContainers);
				break;
			default:
				newminimapObject = Instantiate(enemy, iconContainers);
				break;
		}

		newminimapObject.Initialize(objectInGame.m_transform);
		minimapObjects.Add(objectInGame, newminimapObject);
	}

	public void RemoveObject(ObjectInGame toRemove)
	{
		if (minimapObjects.ContainsKey(toRemove))
			Destroy(minimapObjects[toRemove].gameObject);

		minimapObjects.Remove(toRemove);
	}

	public static Vector3 TranslateToMapPosition(Vector3 position)
	{
		return new Vector3(position.x *scaleRatio, position.z * scaleRatio, 0);
	}
}

