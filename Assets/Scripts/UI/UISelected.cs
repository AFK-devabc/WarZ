using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelected : MonoBehaviour,IPointerClickHandler
{
	public Action<UISelected> selectedAction;

	public string sceneName;

	[SerializeField] private TMP_Text name;
	[SerializeField] private TMP_Text description;

	public void Initialize(string name, string scenename,string description, Action<UISelected> i_selectedAction)
	{
		this.name.text = name;
		this.sceneName = scenename;
		this.description.text = description;
		selectedAction = i_selectedAction;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		selectedAction.Invoke(this);
	}
}
