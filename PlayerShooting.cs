using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Gun gun;
    public Transform gunHolder;
    public Transform gunCenteredHolder;
    public bool centered = false;
    private bool isHoldingShoot = false;
    public PlayerMovement player;
    public Camera cam;
    public UIManager ui;

    public float orgZoomFOV;
    public float zoom;
    public float smoothSpeed;

    public float zoomSniper;

    public float snipeSens = 5f;
    public float middleSens = 25f;
    public float normSens = 50f;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        cam = GetComponentInChildren<Camera>();
        orgZoomFOV = cam.fieldOfView;
    }

    public void OnShoot()
    {
        isHoldingShoot = true;
    }

    public void OnShootRelease()
    {
        isHoldingShoot = false;
    }

    public void OnReload()
    {
        if (gun != null)
        {
            gun.TryReload(ui);
        }
    }

    void Update()
    {
        if (isHoldingShoot && gun != null)
        {
            gun.Shoot(ui);
        }

        if (centered)
        {
            if(gun.isSniper)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, orgZoomFOV - zoomSniper, smoothSpeed * Time.deltaTime);
                if (CompareTag("Player"))
                {
                    GetComponent<PlayerView>().mouseSensitivity = snipeSens;
                }
                else
                {
                    GetComponent<PlayerView>().mouseSensitivity = snipeSens * 2;
                }
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, orgZoomFOV - zoom, Time.deltaTime * smoothSpeed);
                if (CompareTag("Player"))
                {
                    GetComponent<PlayerView>().mouseSensitivity = middleSens;
                }
                else
                {
                    GetComponent<PlayerView>().mouseSensitivity = middleSens * 2;
                }
            }
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, orgZoomFOV, Time.deltaTime * smoothSpeed);
            if (CompareTag("Player"))
            {
                GetComponent<PlayerView>().mouseSensitivity = normSens;
            }
            else
            {
                GetComponent<PlayerView>().mouseSensitivity = normSens * 2;
            }
            /*
            if (!player.isCrouching)
            {
                player.moveSpeed = player.orgMoveSpeed;
            }
            else
            {
                player.moveSpeed = 0.5f*player.orgMoveSpeed;
            }
            */
        }
    }

    /*public void OnDrop()
    {
        if (gun != null)
        {
            gun.Drop();
            gun = null;
            centered = false;
        }
    }*/

    public void OnCenter()
    {
        if (gun != null && !gun.isReloading)
        {
            centered = !centered;
            if (centered)
            {
                //player.moveSpeed = 0.5f * player.orgMoveSpeed;
                gun.transform.position = new Vector3(gunCenteredHolder.position.x, gunCenteredHolder.position.y, gunCenteredHolder.position.z);
                gun.initialPosition = gun.transform.localPosition;
                gun.initialRotation = gun.transform.localRotation;
            }
            else
            {
                gun.transform.position = new Vector3(gunHolder.position.x, gunHolder.position.y, gunHolder.position.z);
                gun.initialPosition = gun.transform.localPosition;
                gun.initialRotation = gun.transform.localRotation;
            }
        }
        else
        {
            //player.moveSpeed = player.orgMoveSpeed;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, orgZoomFOV, Time.deltaTime * smoothSpeed);
        }
    }
}
