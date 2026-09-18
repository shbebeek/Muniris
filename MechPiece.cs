using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechPiece : MonoBehaviour
{
    public int health;
    public EnemyMech mech;
    public Material orgMat;
    public Material hitMat;
    public bool isWalker;
    public Renderer rend;
    public bool isDead;
    public GameObject explosionPrefab;

    void Start()
    {
        orgMat = mech.origMat;
        hitMat = mech.hitMat;
        rend = GetComponent<Renderer>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage"))
        {
            int damage = 0;

            if (other.GetComponent<Bullet>() != null)
            {
                damage = other.GetComponent<Bullet>().damage;
            }
            else if (other.GetComponent<ChakramBullet>() != null)
            {
                damage = other.GetComponent<ChakramBullet>().damage;
            }

            health -= damage;
            AudioManager.instance.PlaySFX(mech.hitSFX, 0.5f);

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
        //anim.enabled = false;
        //anim.SetBool("dead", true);
        //goLimp(true);
        isDead = true;
        rend.material = hitMat;
        foreach(Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Untagged";
        }
        gameObject.layer = 0;
        gameObject.tag = "Untagged";
        if (isWalker)
        {
            mech.canMove = false;
        }
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);
    }

    public IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = orgMat;
    }
}
