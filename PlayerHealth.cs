using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public AudioClip hitSFX;
    public Inventory inv;
    public UIManager ui;
    public PlayerView view;

    public void Start()
    {
        inv = GetComponent<PlayerMovement>().inv;
        health = GetComponent<PlayerMovement>().inv.maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerDamage")
        {
            int damage = collision.gameObject.GetComponent<Bullet>().damage;
            DecreaseHealth(damage);
        }
    }

    public void DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;
        view.AddShake(0.1f,0.25f);
        ui.InstantiateHitUI();
        AudioManager.instance.PlaySFX(hitSFX);
        ui.SetHealthValue(health);

        if(health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void IncreaseHealth(int increaseAmount)
    {
        if(health > (inv.maxHealth - increaseAmount))
        {
            health = inv.maxHealth;
        }
        else
        {
            health += increaseAmount;
        }
        ui.SetHealthValue(health);
    }

    public IEnumerator Die()
    {
        if (!this.enabled)
        {
            yield break;
        }
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().angularDrag = 0f;
        GetComponent<PlayerMovement>().OnCrawl();
        //transform.rotation = Quaternion.Euler(transform.rotation.x-30f, transform.rotation.y, transform.rotation.z);
        GetComponent<Rigidbody>().AddTorque(new Vector3(10f, 10f, 10f));
        GetComponent<InventoryMenu>().enabled = false;

        if (CompareTag("Player"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else{
            //enable invmenu cursor
        }

        ui.EnableDeathUI();

        yield return new WaitForSeconds(0.5f);

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;
        GetComponent<LeftHand>().enabled = false;
        GetComponent<Rigidbody>().angularDrag = 1f;
        this.enabled = false;
    }

    public void Respawn()
    {
        health = inv.maxHealth;
        ui.SetHealthValue(health);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().angularDrag = 10f;

        if (GetComponent<PlayerMovement>().isCrawling)
        {
            GetComponent<PlayerMovement>().OnCrawl();
            GetComponent<PlayerMovement>().OnCrouch();
        }

        if (CompareTag("Player"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //disable invmenu cursor
        }

        GetComponent<InventoryMenu>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerShooting>().enabled = true;
        GetComponent<LeftHand>().enabled = true;
        this.enabled = true;
    }

    public void OnPickUp()
    {
        if (CompareTag("PlayerController"))
        {
            if(health <= 0)
            {
                ui.RestartGame();
            }
        }
    }

    public void OnInventory()
    {
        if (CompareTag("Player"))
        {
            if (health <= 0)
            {
                ui.RestartGame();
            }
        }
    }
}
