using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTurret : Enemy
{
    public Transform[] patrolPoints;

    public float velocity;
    public float angularVelocity;
    public bool isCircular;

    public void Start()
    {
        currentTarget = patrolPoints[0].position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        origMat = rend.material;

        //agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        conTransform = GameObject.FindWithTag("PlayerController").GetComponent<Transform>();
        raycastPosition = transform.Find("RaycastPosition");
    }

    public override void Update()
    {
        ControlMovement();
        base.Update();
    }

    public void ControlMovement()
    {
        if (!isCircular)
        {
            if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
            {
                currentPointIndex++;
                currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, velocity * Time.deltaTime);
            }
        }
        else
        {
            transform.RotateAround(currentTarget, Vector3.up, velocity * Time.deltaTime);
        }
    }
}
