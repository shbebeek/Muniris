using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public GameObject keyBlackFade;
    public GameObject conBlackFade;
    public Transform keyRespawnPoint;
    public Transform conRespawnPoint;

    public void Start()
    {
        keyBlackFade.GetComponent<Image>().CrossFadeAlpha(0f, 1f, false);
        conBlackFade.GetComponent<Image>().CrossFadeAlpha(0f, 1f, false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeAndTransport(keyBlackFade, true));
        }
        else if (other.gameObject.CompareTag("PlayerController"))
        {
            StartCoroutine(FadeAndTransport(conBlackFade, false));
        }
    }

    public IEnumerator FadeAndTransport(GameObject fadeObject, bool isKey)
    {
        fadeObject.GetComponent<Image>().CrossFadeAlpha(1f, 1f, false);

        yield return new WaitForSeconds(1f);

        if (isKey)
        {
            GameObject.FindWithTag("Player").transform.position = keyRespawnPoint.position;
        }
        else
        {
            GameObject.FindWithTag("PlayerController").transform.position = conRespawnPoint.position;
        }

        fadeObject.GetComponent<Image>().CrossFadeAlpha(0f, 1f, true);
    }
}
