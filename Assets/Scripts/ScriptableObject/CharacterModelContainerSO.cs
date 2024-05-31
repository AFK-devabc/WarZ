using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;


 [CreateAssetMenu(menuName = "Container/Character container")]
public class CharacterModelContainerSO : ScriptableObject
{
	[SerializeField] public List<GameObject> m_characterModel;
}
