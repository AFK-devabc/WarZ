using System;
using UnityEngine;

public class ObjectPoolController : MonoBehaviour
{
	public Action<ObjectPoolController> killAction;
	public string id = "";
	public int numberPrewarm = 5;
	public void Init(Action<ObjectPoolController> killAction)
	{
		this.killAction = killAction;
	}

	public void KillObject()
	{
		killAction.Invoke(this);
	}

}
