using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramBullet: MonoBehaviour
{
    public float lifetime = 3f;
    public float speed = 10f;
    public float angularSpeed = 5f;
    public int damage = 50;

    public Rigidbody rb;
    public bool canAdjust;
    public float reactTime = 1.5f;
    public float isBackward = -1f;

    public GameObject player;
    public Transform playerTransform;
    public Vector3 rotationMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = -transform.right * speed;
        StartCoroutine(ReturnToPlayer());
        rotationMovement = new Vector3(0f, 0f, angularSpeed);
    }

    void Update()
    {
        if (canAdjust)
        {
            rb.velocity = Vector3.zero;
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
        }
        transform.Rotate(rotationMovement * Time.deltaTime, Space.Self);
    }

    public IEnumerator ReturnToPlayer()
    {
        yield return new WaitForSeconds(reactTime);
        canAdjust = true;

        yield return new WaitForSeconds(lifetime);
        if (player.CompareTag("Player"))
        {
            GameObject parent = GameObject.FindWithTag("Player").GetComponentInChildren<AzureChakram>().gameObject;

            parent.GetComponent<MeshRenderer>().enabled = true;
            parent.GetComponent<AzureChakram>().isReady = true;
            Destroy(gameObject);
        }
        else
        {
            GameObject parent = GameObject.FindWithTag("PlayerController").GetComponentInChildren<AzureChakram>().gameObject;

            parent.GetComponent<MeshRenderer>().enabled = true;
            parent.GetComponent<AzureChakram>().isReady = true;
            Destroy(gameObject);
        }
    }


    public void SetPlayer(GameObject user)
    {
        player = user;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check if collided with correct player
        if (canAdjust)
        {
            if (collision.gameObject.CompareTag("Player") && player.CompareTag("Player"))
            {
                GameObject parent = GameObject.FindWithTag("Player").GetComponentInChildren<AzureChakram>().gameObject;

                parent.GetComponent<MeshRenderer>().enabled = true;
                parent.GetComponent<AzureChakram>().isReady = true;
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("PlayerController") && player.CompareTag("PlayerController"))
            {
                GameObject parent = GameObject.FindWithTag("PlayerController").GetComponentInChildren<AzureChakram>().gameObject;

                parent.GetComponent<MeshRenderer>().enabled = true;
                parent.GetComponent<AzureChakram>().isReady = true;
                Destroy(gameObject);
            }
        }
    }
}
