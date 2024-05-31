using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoaderBarController : MonoBehaviour
{
	[SerializeField] private Slider m_LoadingSlider;
	[SerializeField] private TMP_Text m_TaskLabel;

	[NonSerialized] public UnityEvent m_OnEnableEvent;


	private void OnEnable()
	{
		m_OnEnableEvent?.Invoke();
	}

	public void LoadTask(string i_TaskName, float i_ProgressValue = 0)
	{
		m_LoadingSlider.value = i_ProgressValue;
		m_TaskLabel.text = i_TaskName;
	}

	public void LoadTask(string i_TaskName, AsyncOperation i_operation)
	{
		m_TaskLabel.text = i_TaskName;
		StartCoroutine(LoadAsynchronously(i_operation));
	}


	IEnumerator LoadAsynchronously(AsyncOperation i_operation)
	{
		while (!i_operation.isDone)
		{
			float progress = Mathf.Clamp01(i_operation.progress / 0.9f);

			m_LoadingSlider.value = progress;	
			yield return null;
		}
	}
}
