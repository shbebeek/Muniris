using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TundraSkybox : MonoBehaviour
{
    public Material tundraSkybox;
    public Material normSkybox;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            Skybox sky = player.transform.Find("Main Camera").GetComponent<Skybox>();
            sky.material = tundraSkybox;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            Skybox sky = player.transform.Find("Main Camera").GetComponent<Skybox>();
            sky.material = normSkybox;
        }
    }
}
