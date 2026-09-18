using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureChest : Interactable
{
    public GameObject content;
    public Inventory playerInventory;
    public bool isOpen;
    public BoolValue storedOpen;
    public usableSignal raiseItem;
    public GameObject[] dialogBox;
    public TextMeshProUGUI[] dialogText;
    public GameObject[] interactBox; // keyboard before player
    public Animator anim;

    public PlayerMovement player;
    public PlayerMovement playerController;
    private PlayerShooting keyboardRightHand;
    private PlayerShooting controllerRightHand;
    public Inventory inventory;
    private InventoryMenu invMenu;
    private InventoryMenu conInvMenu;
    private LeftHand keyboardLeftHand;
    private LeftHand controllerLeftHand;
    
    public bool isKey;
    public bool isLocked;
    public GameObject keyObject;
    public bool canUse = true;

    void Start()
    {
        if(anim == null)
        {
            anim = GetComponent<Animator>();
        }
        isOpen = storedOpen.RuntimeValue;
        canUse = !storedOpen.RuntimeValue;
        keyboardLeftHand = GameObject.FindWithTag("Player").GetComponent<LeftHand>();
        keyboardRightHand = GameObject.FindWithTag("Player").GetComponent<PlayerShooting>();
        controllerLeftHand = GameObject.FindWithTag("PlayerController").GetComponent<LeftHand>();
        controllerRightHand = GameObject.FindWithTag("PlayerController").GetComponent<PlayerShooting>();
        invMenu = keyboardRightHand.GetComponent<InventoryMenu>();
        conInvMenu = controllerRightHand.GetComponent<InventoryMenu>();

        if (isOpen)
        {
            anim.SetBool("isOpen", true);
            anim.Play("FancyChestOpen");
            string org = content.name;
            string rem = " (clone)";
            string weaponName = org.Replace(rem, string.Empty);
            invMenu.EnableObject(weaponName);
            conInvMenu.EnableObject(weaponName);
        }
    }

    void Update()
    {
        if (((player.playerInput.OnFoot.PickUp.WasPressedThisFrame() && playerInRange) || (playerController.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && playerControllerInRange)) && canUse)
        {
            if (!isLocked)
            {
                if (!isOpen)
                {
                    //Open the chest
                    StartCoroutine(OpenChest());
                }
                else
                {
                    //Chest is already open
                    ChestAlreadyOpen();
                }
            }
            else
            {
                if(inventory.numberOfKeys >= 1)
                {
                    inventory.numberOfKeys--;
                    if (!isOpen)
                    {
                        //Open the chest
                        StartCoroutine(OpenLockedChest());
                    }
                    else
                    {
                        //Chest is already open
                        ChestAlreadyOpen();
                    }
                }
                else
                {
                    StartCoroutine(SayLocked());
                }
            }
        }
    }

    public IEnumerator OpenLockedChest()
    {
        canUse = false;
        keyObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        keyObject.SetActive(false);
        StartCoroutine(OpenChest());
    }

    public IEnumerator OpenChest()
    {
        anim.SetBool("isOpen", true);
        anim.Play("FancyChestOpen");
        yield return new WaitForSeconds(0.5f);

        // display item
        if (isKey)
        {
            foreach (TextMeshProUGUI dialog in dialogText)
            {
                dialog.text = "You got a small key! Find a good place to use it...";
            }
            playerInventory.numberOfKeys++;
        }
        else if (content.CompareTag("Item"))
        {
            Item contents = content.GetComponent<Item>();
            foreach(TextMeshProUGUI dialog in dialogText)
            {
                dialog.text = contents.itemDescription;
            }
            playerInventory.AddItem(contents);
        }
        else if (content.CompareTag("LeftHandItem"))
        {
            LeftHandItem contents = content.GetComponent<LeftHandItem>();
            foreach (TextMeshProUGUI dialog in dialogText)
            {
                dialog.text = contents.itemDescription;
            }
            playerInventory.AddLeftHandItem(content);
        }
        else if (content.CompareTag("Gun"))
        {
            Gun contents = content.GetComponent<Gun>();
            foreach (TextMeshProUGUI dialog in dialogText)
            {
                dialog.text = contents.itemDescription;
            }
            playerInventory.AddRightHandItem(content);
        }
        else if (content.CompareTag("Coin"))
        {
            Coin contents = content.GetComponent<Coin>();
            if (contents.isTap)
            {
                foreach (TextMeshProUGUI dialog in dialogText)
                {
                    dialog.text = "You got " + contents.weight + " taps!";
                }
                playerInventory.taps += contents.weight;
            }
            else
            {
                foreach (TextMeshProUGUI dialog in dialogText)
                {
                    dialog.text = "You got " + contents.weight + " joules!";
                }
                playerInventory.joules += contents.weight;
            }
        }
        else if (content.CompareTag("StatUpgrade"))
        {
            StatUpgrade contents = content.GetComponent<StatUpgrade>();
            contents.ApplyUpgrade();
            foreach (TextMeshProUGUI dialog in dialogText)
            {
                dialog.text = contents.statDescription;
            }
        }

        foreach (GameObject box in dialogBox)
        {
            box.SetActive(true);
        }

        //raiseItem.Raise();
        //context.Raise();
        isOpen = true;
        storedOpen.RuntimeValue = isOpen;


        // pick up script
        if(content != null && !content.CompareTag("StatUpgrade"))
        {
            if (content.CompareTag("LeftHandItem"))
            {
                GameObject itemHolder = GameObject.Find("ItemHolder");
                GameObject conHolder = GameObject.Find("ControllerItemHolder");

                foreach (Transform child in itemHolder.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in conHolder.transform)
                {
                    Destroy(child.gameObject);
                }

                GameObject newWeapon = Instantiate(content, keyboardLeftHand.itemHolder);
                keyboardLeftHand.currentItemScript = newWeapon.GetComponent<LeftHandItem>();
                newWeapon.transform.localPosition = keyboardLeftHand.currentItemScript.initialPosition;
                newWeapon.transform.localRotation = keyboardLeftHand.currentItemScript.initialRotation;

                GameObject newConWeapon = Instantiate(content, controllerLeftHand.itemHolder);
                controllerLeftHand.currentItemScript = newConWeapon.GetComponent<LeftHandItem>();
                newConWeapon.transform.localPosition = controllerLeftHand.currentItemScript.initialPosition;
                newConWeapon.transform.localRotation = controllerLeftHand.currentItemScript.initialRotation;

                inventory.SetCurrentHandItem(true, newWeapon);
                inventory.AddLeftHandItem(newWeapon);
            }
            else if (content.CompareTag("Gun"))
            {
                GameObject gunHolder = GameObject.Find("GunHolder");
                foreach (Transform child in gunHolder.transform)
                {
                    Destroy(child.gameObject);
                }

                GameObject gunConHolder = GameObject.Find("ControllerGunHolder");
                foreach (Transform child in gunConHolder.transform)
                {
                    Destroy(child.gameObject);
                }

                GameObject newWeapon = Instantiate(content, keyboardRightHand.gunHolder);
                newWeapon.transform.localPosition = newWeapon.GetComponent<Gun>().initialPosition;
                newWeapon.transform.localRotation = newWeapon.GetComponent<Gun>().initialRotation;
                keyboardRightHand.gun = newWeapon.GetComponent<Gun>();

                GameObject newConWeapon = Instantiate(content, controllerRightHand.gunHolder);
                newConWeapon.transform.localPosition = newConWeapon.GetComponent<Gun>().initialPosition;
                newWeapon.transform.localRotation = newConWeapon.GetComponent<Gun>().initialRotation;
                controllerRightHand.gun = newConWeapon.GetComponent<Gun>();

                inventory.SetCurrentHandItem(false, newWeapon);
                inventory.AddRightHandItem(newWeapon);
            }

            string org = content.name;
            string rem = " (clone)";
            string weaponName = org.Replace(rem, string.Empty);
            invMenu.EnableObject(weaponName);
            conInvMenu.EnableObject(weaponName);
        }

        yield return new WaitForSeconds(3f);

        foreach (GameObject box in dialogBox)
        {
            box.SetActive(false);
        }
    }

    public void ChestAlreadyOpen()
    {
        foreach (GameObject box in dialogBox)
        {
            box.SetActive(false);
        }
        //raiseItem.Raise();
    }

    public IEnumerator SayLocked()
    {
        foreach (TextMeshProUGUI dialog in dialogText)
        {
            dialog.text = "This chest is locked, find a small key to open it.";
        }
        foreach (GameObject box in dialogBox)
        {
            box.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        foreach (GameObject box in dialogBox)
        {
            box.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //context.Raise();
            playerInRange = true;
            interactBox[0].SetActive(true);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            //context.Raise();
            playerControllerInRange = true;
            interactBox[1].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //context.Raise();
            playerInRange = false;
            interactBox[0].SetActive(false);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            //context.Raise();
            playerControllerInRange = false;
            interactBox[1].SetActive(false);
        }
    }
}
