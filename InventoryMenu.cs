using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{
    public bool invOpen = false;
    public GameObject invMenu;
    public GameObject player;
    public GameObject itemPanel;
    public GameObject questPanel;
    public GameObject statPanel;
    public GameObject item;
    public Inventory inv;

    private PlayerShooting playerGun;
    private LeftHand leftHand;

    public Text tapsText;
    public Text joulesText;
    public Text keysText;

    public GameObject cursor;
    private RectTransform cursorRect;
    private Canvas canvas;
    private Vector2 cursorMovement;
    private RectTransform parentPanel;

    public void Start()
    {
        player = this.gameObject;
        playerGun = this.GetComponent<PlayerShooting>();
        leftHand = this.GetComponent<LeftHand>();

        cursorRect = cursor.GetComponent<RectTransform>();
        canvas = GameObject.Find("PlayerControllerUI").GetComponent<Canvas>();
        parentPanel = invMenu.GetComponent<RectTransform>();
    }

    public void OnInventory()
    {
        if (!this.enabled)
        {
            return;
        }

        invOpen = !invOpen;
        if (invOpen == true)
        {
            //Time.timeScale = 0;
            invOpen = true;

            if (CompareTag("Player"))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            invMenu.SetActive(true);
            player.GetComponent<PlayerMovement>().enabled = false;

            if (player.CompareTag("PlayerController"))
            {
                player.GetComponent<PlayerMovement>().playerControllerInput.GroundTraversal.Disable();
                player.GetComponent<PlayerMovement>().playerControllerInput.InventoryMenu.Enable();
            }

            player.GetComponent<PlayerMovement>().moveDisabled = true;
        }
        else
        {
            player.GetComponent<PlayerMovement>().enabled = true;
            invMenu.SetActive(false);

            if (CompareTag("Player"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            invOpen = false;
            //Time.timeScale = 1;

            if (player.CompareTag("PlayerController"))
            {
                player.GetComponent<PlayerMovement>().playerControllerInput.GroundTraversal.Enable();
                player.GetComponent<PlayerMovement>().playerControllerInput.InventoryMenu.Disable();
            }

            player.GetComponent<PlayerMovement>().moveDisabled = false;
        }
    }

    public void Update()
    {
        tapsText.text = "" + inv.taps;
        joulesText.text = "" + inv.joules;
        keysText.text = "" + inv.numberOfKeys;

        if (player.CompareTag("PlayerController"))
        {
            if (invOpen)
            {
                cursor.SetActive(true);

                CursorMovement();

                if (player.GetComponent<PlayerMovement>().playerControllerInput.InventoryMenu.Select.WasPressedThisFrame())
                {
                    CursorClick();
                }
            } else
            {
                cursor.SetActive(false);
            }
        }
    }

    public void ShowItem()
    {
        itemPanel.SetActive(true);
        questPanel.SetActive(false);
        statPanel.SetActive(false);
    }

    public void ShowQuest()
    {
        itemPanel.SetActive(false);
        questPanel.SetActive(true);
        statPanel.SetActive(false);
    }

    public void ShowStat()
    {
        itemPanel.SetActive(false);
        questPanel.SetActive(false);
        statPanel.SetActive(true);
    }

    public void PickGunItem(GameObject weaponPrefab)
    {
        if (playerGun.centered)
        {
            playerGun.OnCenter();
        }

        GameObject gunHolder = null;
        if (this.CompareTag("PlayerController"))
        {
            gunHolder = GameObject.Find("ControllerGunHolder");
        }else if (this.CompareTag("Player"))
        {
            gunHolder = GameObject.Find("GunHolder");
        }

        foreach (Transform child in gunHolder.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newWeapon = Instantiate(weaponPrefab, playerGun.gunHolder);
        newWeapon.transform.localPosition = weaponPrefab.GetComponent<Gun>().initialPosition;
        newWeapon.transform.localRotation = weaponPrefab.GetComponent<Gun>().initialRotation;

        playerGun.gun = newWeapon.GetComponent<Gun>();
        inv.SetCurrentHandItem(false, newWeapon);
    }

    public void PickLeftHandItem(GameObject itemPrefab)
    {
        GameObject itemHolder = null;
        if (this.CompareTag("PlayerController"))
        {
            itemHolder = GameObject.Find("ControllerItemHolder");
        }
        else if (this.CompareTag("Player"))
        {
            itemHolder = GameObject.Find("ItemHolder");
        }
       
        foreach (Transform child in itemHolder.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newItem = Instantiate(itemPrefab, leftHand.itemHolder);
        newItem.transform.localPosition = newItem.GetComponent<LeftHandItem>().initialPosition;
        newItem.transform.localRotation = newItem.GetComponent<LeftHandItem>().initialRotation;

        leftHand.currentItemScript = newItem.GetComponent<LeftHandItem>();
        inv.SetCurrentHandItem(true, newItem);
    }

    public void EnableObject(string objectName)
    {
        // button name needs to match weapon prefab name!
        Transform button = itemPanel.transform.Find(objectName);

        if(button != null)
        {
            button.gameObject.SetActive(true);
        }
        else
        {
            print("notfound");
        }
    }

    public void CursorMovement()
    {
        Vector3 moveDelta = new Vector3(cursorMovement.x, cursorMovement.y, 0) * 500f * Time.deltaTime;
        cursorRect.anchoredPosition += (Vector2)moveDelta;

        // clamp cursor within canvas
        Vector2 clampedPosition = cursorRect.anchoredPosition;
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasRect.width/2 - 200, canvasRect.width / 2 - 200);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasRect.height / 2 + 100, canvasRect.height / 2 + 100);
        cursorRect.anchoredPosition = clampedPosition;
    }

    public void OnMovement(InputValue value)
    {
        cursorMovement = value.Get<Vector2>();
    }

    public void CursorClick()
    {
        if(EventSystem.current == null)
        {
            print("not found");
            return;
        }

        // convert cursor anchored pos to screen pos
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera,
            cursorRect.position);

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach(RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponentInParent<Button>();

            if(button != null && button.interactable)
            {
                Debug.Log("Clicked: " + button.name);

                button.onClick.Invoke();

                return;
            }
        }

        Debug.Log("no button found");
    }
}
