using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class NetworkPlayer : NetworkBehaviour
{
	private int m_WeaponIndex;
	private int m_CharacterIndex;

	[SerializeField] private Transform modelHolder;
	[SerializeField] private Transform weaponHolder;

	[SerializeField] private CharacterModelContainerSO m_modelContainerSO;
	[SerializeField] private WeaponContainerSO m_weaponContainerSO;

	public Weapon currentWeapon;

	[SerializeField] public PlayerController playerController;
	[SerializeField] public PlayerAttack playerAttack;
	[SerializeField] public PlayerInput playerInput;
	[SerializeField] public BaseHealthBehavior healthBehavior;
	[SerializeField] Transform playerTransform;

	ObjectPoolingManager poolingManager;

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

	}

	[ClientRpc]
	private void SetupCharacterDoneClientRpc(string modelName, string weaponName, int weaponIndex, int modelIndex)
	{
		m_WeaponIndex = weaponIndex;
		m_CharacterIndex = modelIndex;
		if (IsLocalPlayer)
		{
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

			Animator modelAnimator = playerTransform.GetComponentInChildren<Animator>();
			currentWeapon = m_weaponContainerSO.weapons[m_WeaponIndex];
			playerAttack.EquipWeapon(currentWeapon);
			playerController.ani = modelAnimator;
			playerAttack.animator = modelAnimator;

			Utils.isClientCharacterSetupDone = true;
			Utils.OnClientCharacterSetupDone?.Invoke(this);
		}
	}

	[ServerRpc]
	private void SetupCharacterServerRpc(int characterIndex, int weaponIndex)
	{
			//healthBehavior.Init(100);

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
		NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(currentWeapon.projectileModel.gameObject, i_position, i_quaternion);

		ProjectileController projectileController = networkObject.GetComponent<ProjectileController>();
		projectileController.ResetState();
		networkObject.Spawn();
		SpawnBulletClientRPC(i_position, i_quaternion);
		//audio.clip = weapon.gunSound;
		//audio.Play();
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
