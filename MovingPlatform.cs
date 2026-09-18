using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public AnimationClip animClip;

    public void Start()
    {
        GetComponent<Animator>().Play(animClip.name);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerController"))
        {
            collision.gameObject.transform.parent = this.transform;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = GameObject.Find("Keyboard Player").transform;
        }

        if (collision.gameObject.CompareTag("PlayerController"))
        {
            collision.gameObject.transform.parent = GameObject.Find("Controller Player").transform;
        }
    }
}
