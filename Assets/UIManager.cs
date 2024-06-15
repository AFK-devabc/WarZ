using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField] private LoaderBarController m_LoaderBar;
	private async void Start()
	{
	}

	public LoaderBarController GetLoaderBar()
	{
		m_LoaderBar.enabled = true;
		return m_LoaderBar;
	}
}

//[CreateAssetMenu(fileName = "UIPrefabsContainer", menuName = "ScriptableObject/UIPrefabsContainer", order = 1)]

//public class UIPrefabsContainer : ScriptableObject
//{
//	public LoaderBarController m_LoaderBar;
//}
