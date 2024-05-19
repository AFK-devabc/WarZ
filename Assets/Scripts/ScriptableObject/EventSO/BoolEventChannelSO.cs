using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Events/Bool Event Channel")]
public class BoolEventChannelSO : ScriptableObject
{
	public UnityAction<bool> OnEventRaised;

	public void RaiseEvent(bool i_value)
	{
		OnEventRaised?.Invoke(i_value);
	}
}
