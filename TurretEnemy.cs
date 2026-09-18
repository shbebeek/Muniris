using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public Transform raycastPosition;
    public LineRenderer lineRenderer;
    public float rayLength = 10f;

    public void Start()
    {
        raycastPosition = GetComponent<Enemy>().raycastPosition;
        lineRenderer = GetComponent<LineRenderer>();
    }
    // used mainly for debug, could be a feature if iterated upon
    public void Update()
    {
        Vector3 startPos = raycastPosition.position;
        Vector3 direction = GameObject.FindWithTag("Player").transform.position - raycastPosition.position;

        if (Physics.Raycast(startPos,direction,out RaycastHit hit, rayLength))
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos + (direction * rayLength));
        }
    }
}
