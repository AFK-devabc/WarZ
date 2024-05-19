using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/String Event Channel")]
public class OneStringEventChannel : ScriptableObject
{
	public UnityAction<string> OnEventRaised;

	public void RaiseEvent(string i_value)
	{
		OnEventRaised?.Invoke(i_value);
	}
}
