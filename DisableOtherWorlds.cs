using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOtherWorlds : MonoBehaviour
{
    public GameObject[] listToEnable;

    public GameObject[] listToDisable;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerController"))
        {
            DisableAll();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerController"))
        {
            EnableAll();
        }
    }

    public void DisableAll()
    {
        foreach (GameObject world in listToEnable)
        {
            world.SetActive(true);
        }


        foreach (GameObject world in listToDisable)
        {
            world.SetActive(false);
        }
    }

    public void EnableAll()
    {
        foreach (GameObject world in listToDisable)
        {
            world.SetActive(true);
        }
    }
}
