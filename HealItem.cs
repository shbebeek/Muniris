using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : LeftHandItem
{
    public PlayerHealth playerHealth;
    public int increaseAmount;
    public GameObject energyFX;

    public override void UseItem(UIManager user)
    {
        StartCoroutine(UseItemDelayed(user));
    }

    public IEnumerator UseItemDelayed(UIManager user)
    {
        isReady = false;
        anim.Play(useClip.name);

        yield return new WaitForSeconds(useClip.length / 2);

        GameObject explosion = Instantiate(energyFX, transform.position, Quaternion.identity);
        float duration = explosion.GetComponent<ParticleSystem>().main.duration;
        Destroy(explosion, duration);

        GameObject.Find("HealthGainSound").GetComponent<AudioSource>().Play();

        if (user.CompareTag("KeyboardUI"))
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
        else
        {
            playerHealth = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerHealth>();
        }
        playerHealth.IncreaseHealth(increaseAmount);

        yield return new WaitForSeconds(useClip.length / 2);

        GameObject.Find("HealthGainSound").GetComponent<AudioSource>().Stop();
        isReady = true;
    }
}
