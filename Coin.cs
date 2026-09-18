using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Inventory playerInventory;
    public bool isTap;
    public int weight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") && !other.isTrigger) || (other.CompareTag("PlayerController") && !other.isTrigger))
        {
            if (isTap)
            {
                playerInventory.taps += weight;
            }
            else
            {
                playerInventory.joules += weight;
            }
            Destroy(this.gameObject);
        }
    }
}
