using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SeekerBullet : Bullet
{
    public GameObject target;
    public bool isEnemyBullet;
    public float angularVelocity;
    public bool canAdjust;
    public float reactTime = 1.5f;
    public float isBackward = -1f;
    public Transform rayStart;
    public GameObject explosionPrefab;
    public BoxCollider explosionCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(InitialMove());
        explosionCollider.enabled = false;
        inventory = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().inv;
    }

    void Update()
    {
        if (canAdjust)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(isBackward * (target.transform.position - transform.position));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularVelocity * Time.deltaTime);
        }
        else
        {
            rb.velocity = transform.forward * isBackward * speed * 50 * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11 && this.CompareTag("Damage"))
        {
            inventory.GainMagic(damage / 2);
        }
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11 && this.CompareTag("Damage"))
        {
            inventory.GainMagic(damage / 2);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            float duration = explosion.GetComponent<ParticleSystem>().main.duration;
            Destroy(explosion, duration);
            Destroy(gameObject);
        }
    }

    public GameObject FindClosest(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Where(obj => Physics.Linecast(transform.position, obj.transform.position, out RaycastHit hit)).OrderBy(obj => (obj.transform.position - transform.position).sqrMagnitude).FirstOrDefault(); // tried physics.linecast but rocket still aims through colliders
    }

    public void FindTarget()
    {
        if (isEnemyBullet)
        {
            float keyDelta = (GameObject.FindWithTag("Player").transform.position - transform.position).sqrMagnitude;
            float conDelta = (GameObject.FindWithTag("PlayerController").transform.position - transform.position).sqrMagnitude;
            if (keyDelta < conDelta)
            {
                target = GameObject.FindWithTag("Player");
            }
            else
            {
                target = GameObject.FindWithTag("PlayerController");
            }
        }
        else
        {
            target = FindClosest("Enemy");
        }
    }

    public void RaycastTarget()
    {
        if (Physics.Raycast(rayStart.position, transform.forward, out RaycastHit hit, 100f))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Transform transtarget = hit.transform;
                target = transtarget.gameObject;
            }
        }
    }

    public void DebugRay()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3 startPos = rayStart.position;
        Vector3 direction = transform.forward * isBackward;

        if (Physics.Raycast(startPos, direction, out RaycastHit hit, 100f))
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos + (direction * 100f));
        }
    }

    public IEnumerator InitialMove()
    {
        //RaycastTarget();
        //DebugRay();
        yield return new WaitForSeconds(reactTime);
        FindTarget();
        if(target != null)
        {
            rb.velocity = Vector3.zero;
            canAdjust = true;
        }
        else
        {
            print("no target");
        }
    }
}
