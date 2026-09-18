using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public usableSignal context;
    public bool playerInRange;
    public bool playerControllerInRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            context.Raise();
            playerInRange = true;
        }
        if(other.CompareTag("PlayerController") && !other.isTrigger)
        {
            context.Raise();
            playerControllerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            context.Raise();
            playerInRange = false;
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            context.Raise();
            playerControllerInRange = false;
        }
    }
}
