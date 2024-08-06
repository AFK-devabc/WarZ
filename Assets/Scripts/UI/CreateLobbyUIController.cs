using System.Collections.Generic;
using UnityEngine;

public class CreateLobbyUIController : MonoBehaviour
{

	[SerializeField] private List<MapData> m_MapData;
	[SerializeField] private RectTransform mapDataHolder;
	[SerializeField] private UISelected mapSelectPrefab;
	[SerializeField] private RectTransform highlightGameobject;

	[SerializeField] private LobbyUIMediator lobbyUIMediator;

	string scenename = "";
	private void Start()
	{
		Initialized();
	}

	public void Initialized()
	{
		for (int i = 0; i < m_MapData.Count; i++)
		{
			UISelected uISelected = Instantiate(mapSelectPrefab, mapDataHolder);
			uISelected.Initialize(m_MapData[i].Name, m_MapData[i].sceneName, m_MapData[i].Description, OnMapSelected);

			if (i == 0)
				OnMapSelected(uISelected);
		}
	}

	public void OnMapSelected(UISelected uISelected)
	{
		highlightGameobject.SetParent(uISelected.transform, false);
		scenename = uISelected.sceneName;
	}

	public void OnCreateButtonClicked()
	{
		lobbyUIMediator.CreateLobby(scenename);
	}
}

[System.Serializable]
public class MapData
{
	public string Name;
	public string sceneName;
	public string Description;
}