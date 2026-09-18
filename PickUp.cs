using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    public Material highlightMaterial;
    private Material[] originalMaterials;
    private MeshRenderer[] meshRenderers;

    public GameObject weaponPrefab;
    public float lookRange = 3f;

    private bool isLookedAt = false;
    private Camera playerCam;
    private PlayerShooting rightHand;
    private LeftHand leftHand;

    public Inventory inventory;
    private InventoryMenu invMenu;
    public LeftHandItem leftItemScript;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }

        rightHand = FindObjectOfType<PlayerShooting>();
        leftHand = FindObjectOfType<LeftHand>();
        playerCam = rightHand.GetComponentInChildren<Camera>();
        invMenu = rightHand.GetComponent<InventoryMenu>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            if (hit.collider.GetComponentInParent<PickUp>() == this)
            {
                if (!isLookedAt)
                    SetLookedAt(true);

                return;
            }
        }

        if (isLookedAt)
        {
            SetLookedAt(false);
        }
    }

    public void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;

        if (lookedAt)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.material = highlightMaterial;
            }
        }
        else
        {
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMaterials[i];
            }
        }
    }

    public void OnPickUp()
    {
        if (!isLookedAt) return;

        if (gameObject.CompareTag("LeftHandItem")){
            GameObject itemHolder = GameObject.Find("ItemHolder");
            foreach (Transform child in itemHolder.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject newWeapon = Instantiate(weaponPrefab, leftHand.itemHolder);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            leftHand.currentItemScript = newWeapon.GetComponent<LeftHandItem>();

            Destroy(gameObject);
            inventory.SetCurrentHandItem(true, newWeapon);
            inventory.AddLeftHandItem(newWeapon);
        }
        else if (gameObject.CompareTag("Gun"))
        {
            GameObject gunHolder = GameObject.Find("GunHolder");
            foreach (Transform child in gunHolder.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject newWeapon = Instantiate(weaponPrefab, rightHand.gunHolder);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            rightHand.gun = newWeapon.GetComponent<Gun>();

            Destroy(gameObject);
            inventory.SetCurrentHandItem(false, newWeapon);
            inventory.AddRightHandItem(newWeapon);
        }

        string org = weaponPrefab.name;
        string rem = " (clone)";
        string weaponName = org.Replace(rem, string.Empty);
        invMenu.EnableObject(weaponName);
    }
}
