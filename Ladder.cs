using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ladder : MonoBehaviour
{
    public bool isOpen;
    public BoolValue storedOpen;
    public GameObject[] dialogBox;
    public TextMeshProUGUI[] dialogText;
    public GameObject[] interactBox; // keyboard before player

    public PlayerMovement player;
    public PlayerMovement playerController;

    public GameObject fullLength;
    public GameObject shortLength;

    public bool playerInRange;
    public bool playerControllerInRange;
    public bool keyClimbing;
    public bool conClimbing;

    public BoxCollider[] shortBox;
    public BoxCollider[] fullBox;

    public int exitDirection = 1;

    public void Start()
    {
        isOpen = storedOpen.RuntimeValue;
        if (isOpen)
        {
            SetDownLadder();
        }
    }

    public void Update()
    {
        // get on ladder
        CheckInteract();

        // climb on ladder
        if (keyClimbing)
        {
            if (player.playerInput.Climbing.Climb.ReadValue<Vector2>().y > 0)
            {
                player.ResetVelocity();
                player.transform.position += transform.up * player.swimSpeed * Time.deltaTime;
            }
            else if (player.playerInput.Climbing.Climb.ReadValue<Vector2>().y < 0)
            {
                player.ResetVelocity();
                player.transform.position -= transform.up * player.swimSpeed * Time.deltaTime;
            }

            if (player.playerInput.Climbing.Disengage.WasPressedThisFrame())
            {
                SetClimbing(false, true);
            }
        }

        if (conClimbing)
        {
            if (playerController.playerControllerInput.Climbing.Climb.ReadValue<Vector2>().y > 0)
            {
                playerController.ResetVelocity();
                playerController.transform.position += transform.up * playerController.swimSpeed * Time.deltaTime;
            }
            else if (playerController.playerControllerInput.Climbing.Climb.ReadValue<Vector2>().y < 0)
            {
                playerController.ResetVelocity();
                playerController.transform.position -= transform.up * playerController.swimSpeed * Time.deltaTime;
            }

            if (playerController.playerControllerInput.Climbing.Disengage.WasPressedThisFrame())
            {
                SetClimbing(false, false);
            }
        }
    }

    

    public void SetClimbing(bool cling, bool keyPlayer)
    {
        if (keyPlayer)
        {
            if (cling)
            {
                keyClimbing = true;
                player.playerInput.Disable();
                player.playerInput.Climbing.Enable();

                player.GetComponent<PlayerMovement>().enabled = false;
                interactBox[0].SetActive(false);
                player.GetComponent<PlayerMovement>().moveDisabled = true;
                player.rb.useGravity = false;
            }
            else
            {
                keyClimbing = false;
                player.playerInput.Disable();
                player.playerInput.OnFoot.Enable();

                player.GetComponent<PlayerMovement>().enabled = true;
                interactBox[0].SetActive(true);
                player.GetComponent<PlayerMovement>().moveDisabled = false;
                player.rb.useGravity = true;
            }
        }
        else
        {
            if (cling)
            {
                conClimbing = true;
                playerController.playerControllerInput.Disable();
                playerController.playerControllerInput.Climbing.Enable();

                playerController.GetComponent<PlayerMovement>().enabled = false;
                interactBox[1].SetActive(false);
                playerController.GetComponent<PlayerMovement>().moveDisabled = true;
                playerController.rb.useGravity = false;
            }
            else
            {
                conClimbing = false;
                playerController.playerControllerInput.Disable();
                playerController.playerControllerInput.GroundTraversal.Enable();

                playerController.GetComponent<PlayerMovement>().enabled = true;
                interactBox[1].SetActive(true);
                playerController.GetComponent<PlayerMovement>().moveDisabled = false;
                playerController.rb.useGravity = true;
            }
        }
    }

    public void CheckInteract()
    {
        if (player.playerInput.OnFoot.PickUp.WasPressedThisFrame() && playerInRange)
        {
            if (!isOpen)
            {
                SetDownLadder();
            }
            else
            {
                // interact with ladder
                if (!keyClimbing)
                {
                    SetClimbing(true, true);
                }
                else
                {
                    SetClimbing(false, true);
                }
            }
        }
        if (playerController.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && playerControllerInRange)
        {
            if (!isOpen)
            {
                SetDownLadder();
            }
            else
            {
                // interact with ladder
                if (!conClimbing)
                {
                    SetClimbing(true, false);
                }
                else
                {
                    SetClimbing(false, false);
                }
            }
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
            if (keyClimbing)
            {
                player.ResetVelocity();
                player.rb.AddForce(new Vector3(0, player.jumpForce / 3, player.jumpForce / 4 * exitDirection), ForceMode.Impulse);
            }
            SetClimbing(false, true);
            interactBox[0].SetActive(false);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            //context.Raise();
            playerControllerInRange = false;
            if (conClimbing)
            {
                playerController.ResetVelocity();
                playerController.rb.AddForce(new Vector3(0, player.jumpForce / 3, player.jumpForce / 4 * exitDirection), ForceMode.Impulse);
            }
            SetClimbing(false, false);
            interactBox[1].SetActive(false);
        }
    }

    public void SetDownLadder()
    {
        // set down the ladder
        fullLength.SetActive(true);
        shortLength.SetActive(false);
        isOpen = true;
        storedOpen.RuntimeValue = isOpen;
        foreach (BoxCollider box in fullBox)
        {
            box.enabled = true;
        }
        foreach (BoxCollider box in shortBox)
        {
            box.enabled = false;
        }
        playerInRange = false;
        playerControllerInRange = false;
        foreach(GameObject box in interactBox)
        {
            box.SetActive(false);
        }
    }
}
