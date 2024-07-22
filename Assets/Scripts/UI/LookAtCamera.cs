using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public Transform camera;
	public Transform transform;

	private void FixedUpdate()
	{
		if (camera == null)
		{
			if (Utils.camera != null)
				camera = Utils.camera;
		}
		else
		{
			transform.forward = -camera.forward;
		}
	}
}
