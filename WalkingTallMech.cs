using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingTallMech : MultipleShooterChaserEnemy
{
    public SkinnedMeshRenderer rend;

    public override void Update()
    {
        LookForPlayer();
        LookForCon();
        switch (state)
        {
            case State.Idle:
                Idle();
                anim.SetInteger("State", 0);
                break;
            case State.Chasing:
                anim.SetInteger("State", 0);
                Chasing();
                break;
            case State.Attacking:
                anim.SetInteger("State", 2);
                Attacking();
                break;
            case State.Patrolling:
                anim.SetInteger("State", 0);
                Patrolling();
                break;
        }

        rb.velocity = Vector3.zero;

        LookAtPlayer();
        LookAtCon();
        SetLastKnownPlayerPosition();
        SetLastKnownConPosition();
    }

    public override void Die()
    {
        if (!this.enabled) return;
        //rb.freezeRotation = false;
        //anim.enabled = false;
        //anim.SetBool("dead", true);
        //goLimp(true);
        agent.enabled = false;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 15f);
        rend.material = hitMat;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Untagged";
        }
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
        anim.SetInteger("State", 2);
        this.enabled = false;
    }

    public override IEnumerator Blink()
    {
        foreach (GameObject hurt in hurtObjects)
        {
            hurt.GetComponent<SkinnedMeshRenderer>().material = hitMat;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (GameObject hurt in hurtObjects)
        {
            hurt.GetComponent<SkinnedMeshRenderer>().material = origMat;
        }
    }
}
