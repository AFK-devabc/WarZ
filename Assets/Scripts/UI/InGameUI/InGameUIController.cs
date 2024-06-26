using UnityEngine;

public class InGameUIController : MonoBehaviour
{
	public static InGameUIController Singleton { get; private set; }

	public void Awake()
	{
		if (Singleton != null && Singleton != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Singleton = this;
		}
	}

	[SerializeField] private HUD_WeaponUI weaponUI;
	[SerializeField] private PlayerHealthbarController playerHealthbarUI;

	public void Initialize(NetworkPlayer networkPlayer)
	{
		weaponUI.Initialize(networkPlayer.playerAttack);
		playerHealthbarUI.Initialize(networkPlayer.healthBehavior);
	}

	private void Start()
	{
		if (Utils.isClientCharacterSetupDone)
			Initialize(Utils.m_localNetworkPlayer);
		else
			Utils.OnClientCharacterSetupDone += Initialize;
	}
}
