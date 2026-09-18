using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public bool isPlaying = false;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(PlaySFXCoroutine(audioClip, volume));
    }

    public void StopSFX(AudioClip audioClip)
    {
        StartCoroutine(PlaySFXCoroutine(audioClip, 0f));
    }

    IEnumerator PlaySFXCoroutine(AudioClip audioClip, float volume = 1f)
    {
        isPlaying = true;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioClip.length * 2f);
        Destroy(audioSource);
        isPlaying = false;
    }
}
