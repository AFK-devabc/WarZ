using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class NetworkPlayer : NetworkBehaviour
{
	private int m_WeaponIndex;
	private int m_CharacterIndex;

	[SerializeField] public Transform modelHolder;
	[SerializeField] private Transform weaponHolder;

	[SerializeField] private CharacterModelContainerSO m_modelContainerSO;
	[SerializeField] private WeaponContainerSO m_weaponContainerSO;
	Animator modelAnimator;
	public Weapon currentWeapon;

	[SerializeField] public PlayerController playerController;
	[SerializeField] public PlayerAttack playerAttack;
	[SerializeField] public PlayerInput playerInput;
	[SerializeField] public BaseHealthBehavior healthBehavior;
	[SerializeField] public Transform playerTransform;

	ObjectPoolingManager poolingManager;
	[SerializeField] private Hub_ObjectInfomation hub_Enemy;
	[SerializeField] private Transform canvasHolder;

	private bool isdead = false;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		playerController.enabled = false;
		playerAttack.enabled = false;
		playerInput.enabled = false;

		if (IsLocalPlayer)
		{
			Utils.m_localNetworkPlayer = this;
			LocalLobbyUser user = ApplicationController.GetInstance().m_LocalLobbyUser;
			m_WeaponIndex = int.Parse(user.WeaponIndex);
			m_CharacterIndex = int.Parse(user.CharacterIndex);

			SetupCharacterServerRpc(m_CharacterIndex, m_WeaponIndex);
		}
	}

	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
		if (IsLocalPlayer)
		{
			Utils.m_localNetworkPlayer = null;
			Utils.isClientCharacterSetupDone = false;
		}
		else
		{
			Utils.RemoveObject(this.transform);
		}
	}

	[ClientRpc]
	private void SetupCharacterDoneClientRpc(string modelName, string weaponName, int weaponIndex, int modelIndex)
	{
		m_WeaponIndex = weaponIndex;
		m_CharacterIndex = modelIndex;


		if (IsLocalPlayer)
		{
			healthBehavior.m_OnZeroHealthEvent += OnClientDead;

			Transform characterModel = Utils.RecursiveFindChild(playerTransform, modelName);
			SyncTransform modelSync = characterModel.gameObject.AddComponent<SyncTransform>();
			modelSync.Init(characterModel, modelHolder);

			Transform weaponModel = Utils.RecursiveFindChild(playerTransform, weaponName);
			SyncTransform weaponSync = weaponModel.gameObject.AddComponent<SyncTransform>();
			weaponSync.Init(weaponModel, weaponHolder);


			weaponHolder.SetParent(Utils.RecursiveFindChild(playerTransform, "Hand_R"));
			weaponHolder.localPosition = Vector3.zero;
			weaponHolder.localRotation = Quaternion.identity;

			playerController.enabled = true;
			playerAttack.enabled = true;
			playerInput.enabled = true;

			modelAnimator = playerTransform.GetComponentInChildren<Animator>();
			currentWeapon = m_weaponContainerSO.weapons[m_WeaponIndex];
			playerAttack.EquipWeapon(currentWeapon);
			playerController.ani = modelAnimator;
			playerAttack.animator = modelAnimator;

			playerTransform.position = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

			Utils.isClientCharacterSetupDone = true;
			Utils.OnClientCharacterSetupDone?.Invoke(this);
		}
		else
		{
			Utils.AddNewObject(this.transform, ObjectType.Ally);
		}

		healthBehavior.InitilizeClientData(0f, null, null, false);

		AddObjectUI();

	}

	[ServerRpc]
	private void SetupCharacterServerRpc(int characterIndex, int weaponIndex)
	{
		healthBehavior.InitializeBaseData(50);
		healthBehavior.m_OnZeroHealthEvent += OnPlayerDead;
		Utils.m_otherPlayer.Add(this);

		Debug.Log("SetUpCharacterServer Called + " + IsLocalPlayer);
		currentWeapon = m_weaponContainerSO.weapons[weaponIndex];
		var weaponInstance = Instantiate(currentWeapon.weaponModel);
		var modelInstance = Instantiate(m_modelContainerSO.m_characterModel[characterIndex]);
		NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
		NetworkObject networkObject2 = modelInstance.GetComponent<NetworkObject>();
		networkObject.SpawnWithOwnership(OwnerClientId);
		networkObject2.SpawnWithOwnership(OwnerClientId);

		if (networkObject.TrySetParent(this.gameObject) && networkObject2.TrySetParent(this.gameObject))
			SetupCharacterDoneClientRpc(modelInstance.name, weaponInstance.name, weaponIndex, characterIndex);
	}

	private void AddObjectUI()
	{
		Hub_ObjectInfomation hub = Instantiate(hub_Enemy, canvasHolder);
		hub.Initialize(healthBehavior, "", 0, true);
	}

	public void OnClientDead()
	{
		healthBehavior.enabled = false;
		playerAttack.enabled = false;
		playerController.enabled = false;
		modelAnimator.SetTrigger("dead");
	}

	#region Spawn bullet from client and server

	public void SpawnBullet(Vector3 i_position, Quaternion i_quaternion)
	{
		if (IsServer || IsHost)
			SpawnBulletServer(i_position, i_quaternion);
		else
			SpawnBulletServerRpc(i_position, i_quaternion);
	}

	public void SpawnBulletServer(Vector3 i_position, Quaternion i_quaternion)
	{
		ProjectileStats stats = currentWeapon.stats[0];

		for (int i = 0; i < stats.projectilePerShoot; i++)
		{
			NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(currentWeapon.projectileModel.gameObject, i_position, i_quaternion * Quaternion.Euler(0, Random.Range(-stats.shootAngle, stats.shootAngle), 0));

			ProjectileController projectileController = networkObject.GetComponent<ProjectileController>();
			projectileController.SetProjectileStats(stats);
			projectileController.ResetState();
			networkObject.Spawn();
		}

		SpawnBulletClientRPC(i_position, i_quaternion);
		//audio.clip = weapon.gunSound;
		//audio.Play();
	}

	public void OnPlayerDead()
	{
		if (isdead) return;

		isdead = true;
		Utils.m_otherPlayer.Remove(this);
		GameStateManager.GetInstance().OnClientDead();
	}

	[ClientRpc]
	public void SpawnBulletClientRPC(Vector3 i_position, Quaternion i_quaternion)
	{
		if (poolingManager == null)
			poolingManager = ObjectPoolingManager.GetInstance();
		EffectObjectPoolController effect = (EffectObjectPoolController)poolingManager.GetObjectInPool(m_weaponContainerSO.weapons[m_WeaponIndex].gunVFX);
		effect.transform.position = i_position;
		effect.transform.rotation = i_quaternion;
		int randomnumber = Random.Range(0, m_weaponContainerSO.weapons[m_WeaponIndex].gunSFXs.Count);

		effect.SetAudioClip(m_weaponContainerSO.weapons[m_WeaponIndex].gunSFXs[randomnumber]);
		effect.PlayEffect();
	}

	[ServerRpc]
	public void SpawnBulletServerRpc(Vector3 i_position, Quaternion i_quaternion)
	{
		SpawnBulletServer(i_position, i_quaternion);
	}
	#endregion //Spawn bullet from client and server

}
