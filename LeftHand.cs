using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    public LeftHandItem currentItemScript;
    public Transform itemHolder;
    public PlayerMovement player;
    public Inventory inventory;
    public UIManager ui;
    public Camera cam;
    private PlayerShooting playerShooting;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        cam = GetComponentInChildren<Camera>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    public void OnLeftHand()
    {
        if (!this.enabled)
        {
            return;
        }

        if(currentItemScript != null)
        {
            if (inventory.currentMagic >= currentItemScript.magicCost && !playerShooting.centered && currentItemScript.isReady)
            {
                currentItemScript.UseItem(ui);
                inventory.currentMagic -= currentItemScript.magicCost;
                ui.SetMagicValue((int)inventory.currentMagic);
            }
        }
    }
}
