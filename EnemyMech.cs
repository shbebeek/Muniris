using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyMech : MultipleShooterChaserEnemy
{
    public Bullet explosiveBulletPrefab;
    public Transform explosiveSpawnPoint;
    public List<MechPiece> weakPoints = new List<MechPiece>();
    public int weakPointNum;
    public new Material origMat;
    public bool canMove = true;

    public override void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        conTransform = GameObject.FindWithTag("PlayerController").GetComponent<Transform>();

        patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointParent.transform).ToArray();
        weakPointNum = weakPoints.Count;

        currentTarget = patrolPoints[0].position;
        canMove = true;
    }

    public override void Update()
    {
        LookForPlayer();
        LookForCon();
        if (!canMove)
        {
            if(state == State.Chasing)
            {
                state = State.Attacking;
            }
            else if(state == State.Patrolling)
            {
                state = State.Idle;
            }
        }

        switch (state)
        {
            case State.Idle:
                Idle();
                anim.SetInteger("State", 0);
                break;
            case State.Chasing:
                Chasing();
                anim.SetInteger("State", 1);
                break;
            case State.Attacking:
                Attacking();
                anim.SetInteger("State", 0);
                break;
            case State.Patrolling:
                Patrolling();
                anim.SetInteger("State", 1);
                break;
        }

        rb.velocity = Vector3.zero;

        LookAtPlayer();
        LookAtCon();
        SetLastKnownPlayerPosition();
        SetLastKnownConPosition();

        foreach(MechPiece point in weakPoints)
        {
            if (point.isDead)
            {
                weakPointNum--;
                weakPoints.Remove(point);
            }
        }

        if(weakPointNum <= 0)
        {
            Die();
        }
    }

    new void OnCollisionEnter(Collision collision)
    {
        // don't want the main mech to be the one taking damage, that is left for the mech pieces
    }

    public override void Die()
    {
        if (!this.enabled) return;
        //rb.freezeRotation = false;
        //anim.enabled = false;
        anim.SetBool("isDead", true);
        anim.SetInteger("State", 2);
        //goLimp(true);
        agent.enabled = false;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 15f);
        //rend.material = hitMat;
        foreach(GameObject gun in guns)
        {
            gun.SetActive(false);
        }
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
        this.enabled = false;
    }

    public override void Shoot(bool isKey)
    {
        Quaternion bulletRotation;

        if (Time.time > lastShotTime + fireRate)
        {
            foreach (GameObject gun in guns)
            {
                if (isKey)
                {
                    Vector3 directionToPlayer = playerTransform.position - gun.transform.position;
                    directionToPlayer.Normalize();
                    bulletRotation = Quaternion.LookRotation(directionToPlayer);
                }
                else
                {
                    Vector3 directionToPlayer = conTransform.position - gun.transform.position;
                    directionToPlayer.Normalize();
                    bulletRotation = Quaternion.LookRotation(directionToPlayer);
                }

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
                    Transform bulletSpawn = gun.transform.Find("BulletSpawnPoint");
                    Instantiate(bulletPrefab, bulletSpawn.position, bulletRotation);
                    Instantiate(weaponFlash, bulletSpawn.position, bulletSpawn.rotation);
                    AudioManager.instance.PlaySFX(shootSFX, 0.125f);
                }
            }
            
            lastShotTime = Time.time;
            
            // missile launch code
            /*if (Random.Range(0, 10) < 2)
            {
                Instantiate(explosiveBulletPrefab, explosiveSpawnPoint.position, Quaternion.Euler(transform.up));
            }*/
        }
    }
}
