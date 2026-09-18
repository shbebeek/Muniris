using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float speed = 50f;
    public int damage = 10;

    public Rigidbody rb;
    public Inventory inventory;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = -transform.right * speed;
        inventory = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().inv;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 11 && this.CompareTag("Damage"))
        {
            inventory.GainMagic(damage / 2);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11 && this.CompareTag("Damage"))
        {
            inventory.GainMagic(damage / 2);
            Destroy(gameObject);
        }
    }
}
