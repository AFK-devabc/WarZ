	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIController : MonoBehaviour
{
	[SerializeField] private WeaponHolderSO weaponsHolder;

	[SerializeField] private Transform weaponContainer;
	[SerializeField] private UIWeaponHolder weaponHolder;

	[SerializeField] private VoidEventChannelSO m_LeaveCustomCharacterEvent;

	[SerializeField] private LobbyCharacter m_localLobbyCharacter;

	private void Start()
	{
		for (int i = 0; i < weaponsHolder.weapons.Count; i++)
		{
			UIWeaponHolder current = Instantiate(weaponHolder, weaponContainer);

			current.Initialize(this,weaponsHolder.weapons[i],i* 360/ weaponsHolder.weapons.Count, i);
		}
	}

	public void OnLeaveButtonClicked()
	{
		m_LeaveCustomCharacterEvent.RaiseEvent();
	}

	public void OnNextCharacterButtonClicked(bool isNext)
	{
		m_localLobbyCharacter.SetNextCharacter();
	}

	public void OnPreviousCharacterButtonClicked(bool isNext)
	{
		m_localLobbyCharacter.SetPreviousCharacter();
	}

	public void SetWeapon( int index)
	{
		m_localLobbyCharacter.SetWeapon(index);
	}

}
