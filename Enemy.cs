using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bloom;
    public float fireRate;
    public float lastShotTime = 0f;
    public GameObject weaponFlash;

    public Animator anim;
    public Rigidbody rb;
    public Material hitMat;
    public Material origMat;
    public Renderer rend;
    public Gun gun;

    //private NavMeshAgent agent;
    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;

    public enum State { Idle, Attacking, AttackingCon }
    public State state = State.Idle;

    public AudioClip shootSFX;
    public AudioClip hitSFX;

    public float idleTimeCounter;
    public Transform playerTransform;
    public Transform conTransform;
    public bool canSeePlayer;
    public bool canSeeCon;
    public Vector3 lastKnownPlayerPosition;

    public GameObject[] hurtObjects;

    public Transform raycastPosition;
    public bool isStationary;
    public GameObject explosionPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        origMat = rend.material;

        //agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        conTransform = GameObject.FindWithTag("PlayerController").GetComponent<Transform>();
        raycastPosition = transform.Find("RaycastPosition");
    }

    public virtual void Update()
    {
        LookForPlayer();
        LookForCon();
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.AttackingCon:
                AttackingCon();
                break;
        }

        rb.velocity = Vector3.zero;
        LookAtPlayer();
        LookAtCon();
        SetLastKnownPlayerPosition();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            int damage = 0;

            if (collision.gameObject.GetComponent<Bullet>() != null)
            {
                damage = collision.gameObject.GetComponent<Bullet>().damage;
            }
            else if (collision.gameObject.GetComponent<ChakramBullet>() != null)
            {
                damage = collision.gameObject.GetComponent<ChakramBullet>().damage;
            }

            health -= damage;
            AudioManager.instance.PlaySFX(hitSFX, 0.5f);

            if (health <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }
    }

    public void Die()
    {
        if (!this.enabled) return;
        //rb.freezeRotation = false;
        //anim.enabled = false;
        //anim.SetBool("dead", true);
        //goLimp(true);
        //agent.enabled = false;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 15f);
        foreach(GameObject renderer in hurtObjects)
        {
            renderer.GetComponent<Renderer>().material = hitMat;
        }
        gameObject.tag = "Untagged";
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Untagged";
        }
        gameObject.layer = 0;
        GameObject explosion = Instantiate(explosionPrefab, bulletSpawnPoint.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
        this.enabled = false;
    }

    public IEnumerator Blink()
    {
        foreach(GameObject obj in hurtObjects)
        {
            obj.GetComponent<Renderer>().material = hitMat;

        }
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject obj in hurtObjects)
        {
            obj.GetComponent<Renderer>().material = origMat;
        }
    }

    public void goLimp(bool isLimp)
    {
        anim.enabled = !isLimp;
        rb.isKinematic = !isLimp;
        if (isLimp)
        {
            gun.Drop();
        }
    }

    public void Idle()
    {
        //agent.ResetPath();
        idleTimeCounter -= Time.deltaTime;

        if (idleTimeCounter < 0)
        {
            idleTimeCounter = idleTime;
        }
    }

    public void Attacking()
    {
        idleTimeCounter = idleTime;
        //agent.ResetPath();

        Shoot();

        if (Vector3.Distance(transform.position, playerTransform.position) > attackDistance || !canSeePlayer)
        {
            state = State.Idle;
        }
    }

    public void AttackingCon()
    {
        idleTimeCounter = idleTime;
        //agent.ResetPath();

        ShootCon();

        if (Vector3.Distance(transform.position, conTransform.position) > attackDistance || !canSeeCon)
        {
            state = State.Idle;
        }
    }

    public void LookForPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - raycastPosition.position;

        if (Physics.Raycast(raycastPosition.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;
            if (canSeePlayer)
            {
                state = State.Attacking;
            }
        }
    }

    public void LookForCon()
    {
        Vector3 directionToPlayer = conTransform.position - raycastPosition.position;

        if (Physics.Raycast(raycastPosition.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == conTransform;
            if (canSeePlayer)
            {
                state = State.AttackingCon;
            }
        }
    }

    public void LookAtPlayer()
    {
        if (canSeePlayer && !isStationary)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }

    public void LookAtCon()
    {
        if (canSeeCon && !isStationary)
        {
            transform.LookAt(new Vector3(conTransform.position.x, transform.position.y, conTransform.position.z));
        }
    }

    public void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    public void Shoot()
    {
        if (Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = playerTransform.position - bulletSpawnPoint.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomJam = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomJam + 87, 0f);

            float randomActualJam = Random.Range(1, 8);
            if (randomActualJam >= 6)
            {

            }
            else
            {
                Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
                Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                AudioManager.instance.PlaySFX(shootSFX, 0.125f);
            }

            lastShotTime = Time.time;
        }
    }

    public void ShootCon()
    {
        if (Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = conTransform.position - bulletSpawnPoint.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomJam = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomJam + 87, 0f);

            float randomActualJam = Random.Range(1, 8);
            if (randomActualJam >= 6)
            {

            }
            else
            {
                Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
                Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                AudioManager.instance.PlaySFX(shootSFX, 0.125f);
            }

            lastShotTime = Time.time;
        }
    }
}
