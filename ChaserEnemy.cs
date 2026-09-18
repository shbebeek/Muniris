using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class ChaserEnemy : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bloom;
    public float fireRate;
    public float lastShotTime = 0f;
    public GameObject weaponFlash;

    protected Animator anim;
    protected Rigidbody rb;
    public Material hitMat;
    protected Material origMat;
    protected Renderer rend;
    public Gun gun;

    protected NavMeshAgent agent;
    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;
    protected float idleTimeCounter;
    protected Transform playerTransform;
    protected Transform conTransform;
    protected bool canSeePlayer;
    protected bool canSeeCon;
    protected Vector3 lastKnownPlayerPosition;
    protected Vector3 lastKnownConPosition;

    public GameObject patrolPointParent;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Idle;

    public AudioClip shootSFX;
    public AudioClip hitSFX;
    public GameObject explosionPrefab;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        origMat = rend.material;
        //goLimp(false);

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        conTransform = GameObject.FindWithTag("PlayerController").GetComponent<Transform>();

        patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointParent.transform).ToArray();
        currentTarget = patrolPoints[0].position;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            int damage = 0;

            if (collision.gameObject.GetComponent<Bullet>() != null)
            {
                damage = collision.gameObject.GetComponent<Bullet>().damage;
            }else if(collision.gameObject.GetComponent<ChakramBullet>() != null)
            {
                damage = collision.gameObject.GetComponent<ChakramBullet>().damage;
            }

            health -= damage;
            AudioManager.instance.PlaySFX(hitSFX, 0.5f);

            if(health <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }
    }

    public virtual void Die()
    {
        if (!this.enabled) return;
        rb.freezeRotation = false;
        //anim.enabled = false;
        //anim.SetBool("dead", true);
        //goLimp(true);
        agent.enabled = false;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z+15f);
        rend.material = hitMat;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Untagged";
        }
        gameObject.tag = "Untagged";
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
        this.enabled = false;
    }

    public virtual IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = origMat;
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

    public virtual void Update()
    {
        LookForPlayer();
        LookForCon();
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Patrolling:
                Patrolling();
                break;
        }

        rb.velocity = Vector3.zero;

        LookAtPlayer();
        LookAtCon();
        SetLastKnownPlayerPosition();
        SetLastKnownConPosition();
    }

    public void Idle()
    {
        agent.ResetPath();
        idleTimeCounter -= Time.deltaTime;

        if(idleTimeCounter < 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }

    public virtual void Chasing()
    {
        idleTimeCounter = idleTime;
        if (canSeePlayer)
        {
            agent.SetDestination(lastKnownPlayerPosition);
        }
        else
        {
            agent.SetDestination(lastKnownConPosition);
        }

        if (health < minChasingHealth)
        {
            state = State.Patrolling;
        }
        else if ((Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && canSeePlayer) || (Vector3.Distance(transform.position, conTransform.position) <= attackDistance && canSeeCon))
        {
            state = State.Attacking;
        }
        else if ((Vector3.Distance(transform.position, playerTransform.position) > maxVisionDistance) && (Vector3.Distance(transform.position,conTransform.position) > maxVisionDistance))
        {
            state = State.Patrolling;
        }
        else if ((Vector3.Distance(transform.position, playerTransform.position) < positionThreshold && !canSeePlayer) || (Vector3.Distance(transform.position, conTransform.position) < positionThreshold && !canSeeCon))
        {
            state = State.Patrolling;
        }

        if (canSeePlayer)
        {
            Shoot(true);
        }
        else
        {
            Shoot(false);
        }
    }

    public virtual void Attacking()
    {
        idleTimeCounter = idleTime;
        agent.ResetPath();

        if (canSeePlayer)
        {
            Shoot(true);
            if (Vector3.Distance(transform.position, playerTransform.position) > attackDistance || !canSeePlayer)
            {
                if (health < minChasingHealth)
                {
                    state = State.Patrolling; // cautious
                }
                else
                {
                    state = State.Chasing;
                }
            }
        }
        else
        {
            Shoot(false);
            if (Vector3.Distance(transform.position, conTransform.position) > attackDistance || !canSeeCon)
            {
                if (health < minChasingHealth)
                {
                    state = State.Patrolling; // cautious
                }
                else
                {
                    state = State.Chasing;
                }
            }
        }
    }

    public virtual void Patrolling()
    {
        if(Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            float chance = Random.Range(0, 100);

            if(chance < 10)
            {
                state = State.Idle;
                return;
            }

            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
        }
        else
        {
            agent.SetDestination(currentTarget);
        }
    }

    public void LookForPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        if(Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;

            if(canSeePlayer && state != State.Attacking)
            {
                state = State.Chasing;
            }
        }
    }

    public void LookForCon()
    {
        Vector3 directionToPlayer = conTransform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeeCon = hit.transform == conTransform;

            if (canSeeCon && state != State.Attacking)
            {
                state = State.Chasing;
            }
        }
    }

    public void LookAtPlayer()
    {
        if (canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }

    public void LookAtCon()
    {
        if (canSeeCon)
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

    public void SetLastKnownConPosition()
    {
        if (canSeeCon)
        {
            lastKnownConPosition = conTransform.position;
        }
    }

    public virtual void Shoot(bool isKey)
    {
        Quaternion bulletRotation;

        if (isKey)
        {
            Vector3 directionToPlayer = playerTransform.position - bulletSpawnPoint.position;
            directionToPlayer.Normalize();
            bulletRotation = Quaternion.LookRotation(directionToPlayer);
        }
        else {
            Vector3 directionToPlayer = conTransform.position - bulletSpawnPoint.position;
            directionToPlayer.Normalize();
            bulletRotation = Quaternion.LookRotation(directionToPlayer);
        }

        if (Time.time > lastShotTime + fireRate)
        {
            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomJam = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomJam + 87, 0f);

            float randomActualJam = Random.Range(1, 8);
            if (randomActualJam == 4)
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
