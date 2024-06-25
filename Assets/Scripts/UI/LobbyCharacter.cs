using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacter : MonoBehaviour
{
	[SerializeField] private CharacterModelContainerSO m_modelContainerSO;
	[SerializeField] private WeaponContainerSO m_weaponContainerSO;

	[SerializeField] private Transform m_modelHolder;
	[SerializeField] private TMP_InputField m_inputField;

	private LocalLobbyUser m_localLobbyUser;

	[SerializeField] private Weapon weapon;
	[SerializeField] private Transform weaponHolder;

	[SerializeField] private Image icon;
	[SerializeField] private Sprite unreadySprite;
	[SerializeField] private Sprite readySprite;
	[SerializeField] private Sprite hostSprite;


	public void Initialized(LocalLobbyUser i_LocalUsers)
	{
		Utils.OnLeaveCustomCharacterEvent+= () =>
		{
			m_inputField.readOnly = false;
		};

		m_localLobbyUser = i_LocalUsers;
		Reset();
	}

	public  void SetNextCharacter()
	{
		RemoveModel();

		m_localLobbyUser.CharacterIndex = (int.Parse(m_localLobbyUser.CharacterIndex) + 1).ToString();
		if(int.Parse(m_localLobbyUser.CharacterIndex) >= m_modelContainerSO.m_characterModel.Count)
			m_localLobbyUser.CharacterIndex = (0).ToString();
		SetCharacterModel(m_modelContainerSO.m_characterModel[int.Parse(m_localLobbyUser.CharacterIndex)]);
	}

	public void SetPreviousCharacter()
	{
		RemoveModel();

		m_localLobbyUser.CharacterIndex = (int.Parse(m_localLobbyUser.CharacterIndex) - 1).ToString();
		if (int.Parse(m_localLobbyUser.CharacterIndex) < 0)
			m_localLobbyUser.CharacterIndex = (m_modelContainerSO.m_characterModel.Count - 1).ToString();
		SetCharacterModel(m_modelContainerSO.m_characterModel[int.Parse(m_localLobbyUser.CharacterIndex)]);
	}

	private void SetCharacterModel(GameObject characterModel)
	{
		GameObject model = Instantiate(characterModel, m_modelHolder);
		SetWeapon(model.transform);
		model.GetComponent<Animator>().runtimeAnimatorController = this.weapon.animator;
	}

	private void SetWeapon(Transform model)
	{
		weaponHolder.SetParent(Utils.RecursiveFindChild(model, "Hand_R"));
		weaponHolder.transform.localPosition = Vector3.zero;
		weaponHolder.transform.localRotation = Quaternion.identity;
	}

	public void SetWeapon(int index)
	{
		m_localLobbyUser.WeaponIndex = index.ToString();
		EquipWeapon(index);
	}

	public void EquipWeapon(int index)
	{
		this.weapon = m_weaponContainerSO.weapons[index];
		m_modelHolder.GetComponentInChildren<Animator>().runtimeAnimatorController = this.weapon.animator;
		OnWeaponEquip();
	}

	private void OnWeaponEquip()
	{
		for (var i = weaponHolder.childCount - 1; i >= 0; i--)
		{
			// only destroy tagged object
			Destroy(weaponHolder.GetChild(i).gameObject);
		}

		GameObject newWeapon = Instantiate(weapon.weaponModel, weaponHolder);
	}

	private void UnSetWeapon()
	{
		weaponHolder.SetParent(m_modelHolder.parent);
	}

	private void RemoveModel()
	{
		UnSetWeapon();

		if (m_localLobbyUser == null)
			return;
		for (var i = m_modelHolder.childCount; i > 0; i--)
		{
			// only destroy tagged object
			Destroy(m_modelHolder.GetChild(i - 1).gameObject);
		}
	}

	private void Reset()
	{
		RemoveModel();

		if (m_localLobbyUser.CharacterIndex != null)
		{
			this.gameObject.SetActive(true);
			m_inputField.text = m_localLobbyUser.DisplayName;
			SetCharacterModel(m_modelContainerSO.m_characterModel[int.Parse(m_localLobbyUser.CharacterIndex)]);
			EquipWeapon(int.Parse(m_localLobbyUser.WeaponIndex));

			if(m_localLobbyUser.IsHost)
			{
				icon.sprite = hostSprite;
			}
			else if(m_localLobbyUser.IsReady== "true")
			{
				icon.sprite = readySprite;
			}
			else
			{
				icon.sprite = unreadySprite;
			}

		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}

	public void OnCharacterNameChanged( string i_characterName)
	{
		m_localLobbyUser.DisplayName = i_characterName;
		Debug.Log(m_localLobbyUser.DisplayName);
	}

	public void OnCharacterClicked()
	{
		Utils.OnEnterCustomCharacterEvent?.Invoke();
		m_inputField.readOnly = true;

	}
}
