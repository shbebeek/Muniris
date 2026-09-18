using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockedDoor : MonoBehaviour
{
    public Inventory playerInventory;
    public bool isOpen;
    public BoolValue storedOpen;
    public GameObject[] dialogBox;
    public TextMeshProUGUI[] dialogText;
    public GameObject[] interactBox; // keyboard before player
    public Animator anim;
    public AnimationClip openClip;

    public bool isLocked;
    public GameObject keyObject;

    public bool keyInRange;
    public bool conInRange;

    public PlayerMovement key;
    public PlayerMovement con;

    public bool canUse = true;

    void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        isOpen = storedOpen.RuntimeValue;
        canUse = !storedOpen.RuntimeValue;

        if (isOpen)
        {
            anim.Play(openClip.name);
        }

        key = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        con = GameObject.FindWithTag("PlayerController").GetComponent<PlayerMovement>();
    }

    public virtual void Update()
    {
        if (((key.playerInput.OnFoot.PickUp.WasPressedThisFrame() && keyInRange) || (con.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && conInRange)) && canUse)
        {
            if (!isLocked)
            {
                if (!isOpen)
                {
                    OpenDoor();
                }
                else
                {
                    DoorAlreadyOpen();
                }
            }
            else
            {
                if (playerInventory.numberOfKeys >= 1)
                {
                    playerInventory.numberOfKeys--;
                    if (!isOpen)
                    {
                        StartCoroutine(OpenLockedDoor());
                    }
                    else
                    {
                        DoorAlreadyOpen();
                    }
                }
                else
                {
                    StartCoroutine(SayLocked());
                }
            }
        }
    }

    public IEnumerator OpenLockedDoor()
    {
        canUse = false;
        keyObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        keyObject.SetActive(false);
        OpenDoor();
    }

    public void OpenDoor()
    {
        isOpen = true;
        storedOpen.RuntimeValue = isOpen;
        anim.Play(openClip.name);
        GameObject.Find("GateOpen").GetComponent<AudioSource>().Play();
    }

    public void DoorAlreadyOpen()
    {
        foreach (GameObject box in dialogBox)
        {
            box.SetActive(false);
        }
    }

    public virtual IEnumerator SayLocked()
    {
        foreach (TextMeshProUGUI dialog in dialogText)
        {
            dialog.text = "This door is locked, find a small key to open it.";
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
        if (other.CompareTag("Player") && !other.isTrigger && canUse)
        {
            //context.Raise();
            keyInRange = true;
            interactBox[0].SetActive(true);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger && canUse)
        {
            //context.Raise();
            conInRange = true;
            interactBox[1].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //context.Raise();
            keyInRange = false;
            interactBox[0].SetActive(false);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            //context.Raise();
            conInRange = false;
            interactBox[1].SetActive(false);
        }
    }
}
