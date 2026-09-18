using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float reloadTime = 0.5f;
    public float fireRate = 0.15f;
    public int magSize = 20;

    public GameObject bullet;
    public Transform bulletSpawnPoint;

    public GameObject weaponFlash;
    public GameObject droppedWeapon;

    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;

    private int currentAmmo;
    public bool isReloading = false;
    private float nextTimeToFire = 0f;

    public Quaternion initialRotation;
    public Vector3 initialPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);
    public Quaternion bulletRotationAdjust = Quaternion.Euler(0, 0, -1f);

    public AudioClip shootSFX;

    public string itemDescription;
    public GameObject reloadObject;

    public bool isSniper;

    void Start()
    {
        currentAmmo = magSize;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    public void Shoot(UIManager user)
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;
        user.ammoText.text = currentAmmo.ToString();

        AudioManager.instance.PlaySFX(shootSFX, 0.25f);

        Quaternion adjustedRotation = bulletSpawnPoint.rotation * bulletRotationAdjust;

        Instantiate(bullet, bulletSpawnPoint.position, adjustedRotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload(user));
            return;
        }
    }

    IEnumerator Reload(UIManager user)
    {
        isReloading = true;
        reloadObject.SetActive(false);

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;

        while(t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfReload);
            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        t = 0f;

        reloadObject.SetActive(true);
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfReload);
            yield return null;
        }

        currentAmmo = magSize;
        user.ammoText.text = currentAmmo.ToString();
        isReloading = false;
    }

    public void TryReload(UIManager user)
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;

        StartCoroutine(Reload(user));
    }

    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initialPosition + new Vector3(recoilDistance, 0, 0);
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initialPosition, recoilTarget, t);
            yield return null;
        }

        t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilTarget, initialPosition, t);
            yield return null;
        }

        transform.localPosition = initialPosition;
    }

    public void Drop()
    {
        UIManager.instance.ammoText.text = "";
        //Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
