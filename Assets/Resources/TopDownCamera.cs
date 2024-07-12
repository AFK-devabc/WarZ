using UnityEngine;

public class TopDownCamera : MonoBehaviour
{

	[Header("Camera Basic Settings")]

	public Camera m_localCamera;

	public float FollowSpeed = 1.0f;
	public Transform TargetToFollow;
	//Offset
	[HideInInspector]
	public float TargetHeight = 0.0f;
	[HideInInspector]
	public float targetHeightOffset = -1.0f;
	//[HideInInspector]
	//public float ActualHeight = 0.0f;

	public float HeightSpeed = 1.0f;
	//public Transform CameraOffset;

	[Header("Shooting Shake Settings")]
	public bool isShaking = false;
	public float shakeFactor = 3f;
	public float shakeTimer = .2f;
	public float shakeSmoothness = 5f;
	[HideInInspector]
	public float actualShakeTimer = 0.2f;

	[Header("Explosion Shake Settings")]
	public bool isExpShaking = false;
	public float shakeExpFactor = 5f;
	public float shakeExpTimer = 1.0f;
	public float shakeExpSmoothness = 3f;
	[HideInInspector]
	public float actualExpShakeTimer = 1.0f;

	[Header("Movement Shake Settings")]
	public float movShaking = 1.0f;
	private Vector3 randomShakePos = Vector3.zero;

	private bool showBlood = true;

	[Header("Minimap")]
	[HideInInspector]
	public GameObject minimap;
	[HideInInspector]
	public bool useMinimap = false;
	[HideInInspector]
	public float minimapZoom = 25;
	private GameObject currentMinimap;

	// Use this for initialization
	protected virtual void Start()
	{

		Utils.camera = m_localCamera.transform;
		actualShakeTimer = shakeTimer;

		if (useMinimap && minimap)
		{
			CreateMinimap();
		}

		this.TargetToFollow = Utils.m_localNetworkPlayer.transform;
		Utils.m_localNetworkPlayer?.GetComponent<PlayerController>().SetCamera(this);
		Utils.m_localNetworkPlayer?.GetComponent<PlayerAttack>().SetCamera(this);
	}

	public void CreateMinimap()
	{
		if (!currentMinimap)
		{
			currentMinimap = Instantiate(minimap);
			currentMinimap.GetComponentInChildren<Camera>().orthographicSize = minimapZoom;
			currentMinimap.transform.SetParent(transform);
			currentMinimap.transform.localRotation = Quaternion.identity;
			currentMinimap.transform.localPosition = new Vector3(0, currentMinimap.transform.position.y, 0);
			currentMinimap.transform.localScale = Vector3.one;

			SendMessageUpwards("NewMinimapCreated", currentMinimap, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			EnableBlood();
		}
	}

	public Vector3 CalculateRandomShake(float shakeFac, bool isExplosion)
	{
		randomShakePos = new Vector3(Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac));
		if (isExplosion)
			return randomShakePos * (actualExpShakeTimer / shakeExpTimer);
		else
			return randomShakePos * (actualShakeTimer / shakeTimer);
	}

	public void Shake(float factor, float duration)
	{
		isShaking = true;
		shakeFactor = factor;
		shakeTimer = duration;
		actualShakeTimer = shakeTimer;
	}

	public void ExplosionShake(float factor, float duration)
	{
		isExpShaking = true;
		shakeExpFactor = factor;
		shakeExpTimer = duration;
		actualExpShakeTimer = shakeExpTimer;
	}

	protected virtual void FixedUpdate()
	{
		if (TargetToFollow)
		{
			transform.position = Vector3.Lerp(transform.position, TargetToFollow.position, FollowSpeed * Time.deltaTime);

			//ActualHeight = Mathf.Lerp(ActualHeight, TargetHeight + targetHeightOffset, Time.deltaTime * HeightSpeed);

			//CameraOffset.localPosition = new Vector3(0.0f, 0.0f, ActualHeight);
		}

		if (isShaking && !isExpShaking)
		{
			if (actualShakeTimer >= 0.0f)
			{
				actualShakeTimer -= Time.deltaTime;
				Vector3 newPos = transform.localPosition + CalculateRandomShake(shakeFactor, false);
				transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, shakeSmoothness * Time.deltaTime);
			}
			else
			{
				isShaking = false;
				actualShakeTimer = shakeTimer;
			}
		}

		else if (isExpShaking)
		{
			if (actualExpShakeTimer >= 0.0f)
			{
				actualExpShakeTimer -= Time.deltaTime;
				Vector3 newPos = transform.localPosition + CalculateRandomShake(shakeExpFactor, true);
				transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, shakeExpSmoothness * Time.deltaTime);
			}
			else
			{
				isExpShaking = false;
				actualExpShakeTimer = shakeExpTimer;
			}
		}

	}

	public void EnableBlood()
	{
		if (showBlood)
		{
			var newMask = GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("Blood"));
			////Debug.Log(GetComponentInChildren<Camera>().cullingMask);
			GetComponentInChildren<Camera>().cullingMask = newMask;
			showBlood = false;
		}
		else if (!showBlood)
		{
			var newMask = GetComponentInChildren<Camera>().cullingMask | (1 << LayerMask.NameToLayer("Blood"));
			// //Debug.Log(GetComponentInChildren<Camera>().cullingMask);
			GetComponentInChildren<Camera>().cullingMask = newMask;
			showBlood = true;
		}

	}
}
