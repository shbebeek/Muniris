using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonPatroller : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private NavMeshAgent agent;

    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public Transform[] patrolPoints;

    public float velocity;
    public float angularVelocity;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        anim.SetBool("FlyingFWD", true);
        currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
    }

    public void Update()
    {
        if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, velocity * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularVelocity * Time.deltaTime);
        }
    }
}
