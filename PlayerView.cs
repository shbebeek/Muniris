using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    public static PlayerView instance;

    public float mouseSensitivity = 100f;
    public Transform cam;

    private float xRotation = 0f;
    private Vector2 lookInput;

    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.1f;
    public float shakeFadeSpeed = 1.5f;
    private Vector3 initialCamPos;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCamPos = cam.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleShake();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

   public void HandleShake()
    {
        if(shakeDuration > 0)
        {
            cam.localPosition = initialCamPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * shakeFadeSpeed;
        }
        else
        {
            cam.localPosition = initialCamPos;
        }
    }

    public void AddShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
