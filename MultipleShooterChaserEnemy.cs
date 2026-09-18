using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShooterChaserEnemy : ChaserEnemy
{
    public GameObject[] guns;
    public GameObject[] hurtObjects;

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
        }
    }

    public override IEnumerator Blink()
    {
        foreach(GameObject hurt in hurtObjects)
        {
            hurt.GetComponent<Renderer>().material = hitMat;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (GameObject hurt in hurtObjects)
        {
            hurt.GetComponent<Renderer>().material = origMat;
        }
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
        this.enabled = false;
    }
}
