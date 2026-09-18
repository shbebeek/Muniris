using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveStation : MonoBehaviour
{
    public Transform keyRespawnTransform;
    public Transform conRespawnTransform;
    public PlayerMovement key;
    public PlayerMovement con;
    public PlayerHealth keyHealth;
    public PlayerHealth conHealth;

    public GameObject[] dialogBox; // key before con
    public TextMeshProUGUI[] dialogText;
    public GameObject[] interactBox; // keyboard before player
    public Animator anim;

    public bool keyInRange;
    public bool conInRange;

    public string message = "Progress saved!";

    void Start()
    {
        key = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        con = GameObject.FindWithTag("PlayerController").GetComponent<PlayerMovement>();
        keyHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        conHealth = GameObject.FindWithTag("PlayerController").GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if(key.playerInput.OnFoot.PickUp.WasPressedThisFrame() && keyInRange)
        {
            keyHealth.health = keyHealth.inv.maxHealth;
            GameObject.FindWithTag("KeyboardUI").GetComponent<UIManager>().SetHealthValue(keyHealth.health);
            key.respawnPoint = keyRespawnTransform;
            StartCoroutine(DisplaySave(0));

        }
        if(con.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && conInRange)
        {
            conHealth.health = conHealth.inv.maxHealth;
            GameObject.FindWithTag("ControllerUI").GetComponent<UIManager>().SetHealthValue(conHealth.health);
            con.respawnPoint = conRespawnTransform;
            StartCoroutine(DisplaySave(1));
        }
    }

    public IEnumerator DisplaySave(int player)
    {
        dialogText[player].text = message;
        dialogBox[player].SetActive(true);
        anim.speed = 10f;
        yield return new WaitForSeconds(1f);
        dialogBox[player].SetActive(false);
        anim.speed = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //context.Raise();
            keyInRange = true;
            interactBox[0].SetActive(true);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
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
