using UnityEngine;

public class SyncTransform : MonoBehaviour
{
	public void Init(Transform i_currentTransform, Transform i_targetTransform)
	{
		currentTransform = i_currentTransform;
		targetTransform = i_targetTransform;
	}
	public Transform currentTransform;
	public Transform targetTransform;
	private void FixedUpdate()
	{
		currentTransform.position = targetTransform.position;
		currentTransform.rotation = targetTransform.rotation;
		currentTransform.localScale = targetTransform.localScale;
	}
}
