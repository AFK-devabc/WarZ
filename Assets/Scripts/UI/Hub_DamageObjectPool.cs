using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hub_DamageObjectPool : ObjectPoolController
{
	[SerializeField] private TMP_Text m_text;
	public void ShowDamage(int damage)
	{
		m_text.text = damage.ToString();
	}
}
