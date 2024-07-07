using System.Collections;
using UnityEngine;

public class EffectObjectPoolController : ObjectPoolController
{
	public ParticleSystem particle;
	public AudioSource audioSource;
	public float lifeTime;

	public void PlayEffect()
	{
		audioSource.Play();
		particle.Play();
		StartCoroutine(KillObjectOnPlay());
	}

	public void SetAudioClip(AudioClip i_clip)
	{
		audioSource.clip = i_clip;
	}

	IEnumerator KillObjectOnPlay()
	{
		yield return new WaitForSeconds(lifeTime);
		KillObject();
	}

}
