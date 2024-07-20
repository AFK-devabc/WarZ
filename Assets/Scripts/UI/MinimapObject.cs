using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MinimapObject : MonoBehaviour
{
	private Transform actualObject;

	[SerializeField] private RectTransform minimapObject;
	[SerializeField] private bool showDirection = false;
	public void Initialize(Transform i_actualObject)
	{
		actualObject = i_actualObject;
	}

	private void FixedUpdate()
	{
		if (actualObject != null)
		{
			minimapObject.localPosition = MinimapController.TranslateToMapPosition(actualObject.position);
			if(showDirection )
			{
				minimapObject.eulerAngles = new Vector3(0f,0f, - actualObject.eulerAngles.y);
			}
		}
	}
}

