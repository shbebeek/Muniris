using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzureChakram : LeftHandItem
{
    public float lifetime = 1f;
    public float speed = 20f;
    public float angularSpeed = 20f;
    public int damage = 50;

    public GameObject player;

    public Transform bulletSpawnPoint;
    public GameObject chakramBullet;
    public Vector3 rotationMovement;

    public override void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        isReady = true;
        rotationMovement = new Vector3(0f, 0f, angularSpeed);
    }

    public override void UseItem(UIManager user)
    {
        if (isReady)
        {
            if (user.CompareTag("KeyboardUI"))
            {
                player = GameObject.FindWithTag("Player");
            }
            else
            {
                player = GameObject.FindWithTag("PlayerController");
            }
            chakramBullet.GetComponent<ChakramBullet>().SetPlayer(player);

            Instantiate(chakramBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            GetComponent<Renderer>().enabled = false;
            isReady = false;
        }
    }

    /*public void Update()
    {
        transform.Rotate(rotationMovement * Time.deltaTime, Space.Self);
    }*/
}