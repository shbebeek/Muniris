using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandItem : MonoBehaviour
{
    public float fireRate = 0.5f;
    public int magicCost = 20;

    public GameObject projectile;
    public Transform projectileSpawnPoint;

    public GameObject projectileFlash;

    public Animation useAnimation;
    public AnimationClip useClip;
    public Animator anim;
    public AudioClip useSFX;

    public Quaternion initialRotation;
    public Vector3 initialPosition;

    public Inventory playerInventory;
    public UIManager ui;

    public string itemDescription;

    public bool isReady = true;

    public virtual void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    public virtual void UseItem(UIManager user)
    {
        
    }
}
