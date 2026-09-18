using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRegion : MonoBehaviour
{
    public AudioSource audioSource;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerController"))
        {
            audioSource.Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerController"))
        {
            audioSource.Stop();
        }
    }
}
