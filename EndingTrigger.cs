using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingTrigger : MonoBehaviour
{
    public Inventory playerInventory;
    public GameObject[] interactBox; // keyboard before player

    private bool keyInRange;
    private bool conInRange;

    private PlayerMovement key;
    private PlayerMovement con;

    public GameObject winScreen;
    public Text completion;
    public Text time;

    public int totalTaps;

    public float trackedTime;

    private bool isGameFinished;

    void Start()
    {
        key = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        con = GameObject.FindWithTag("PlayerController").GetComponent<PlayerMovement>();
        trackedTime = 0f;
    }

    public virtual void Update()
    {
        if ((key.playerInput.OnFoot.PickUp.WasPressedThisFrame() && keyInRange) || (con.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && conInRange))
        {
            FinishGame();
        }
    }

    public void FinishGame()
    {
        GameObject.Find("EndingAnthem").GetComponent<AudioSource>().Play();
        isGameFinished = true;
        keyInRange = false;
        conInRange = false;

        key.moveDisabled = true;
        con.moveDisabled = true;
        winScreen.SetActive(true);

        double tapsPercent = Mathf.Round(((float)playerInventory.taps / totalTaps) * 100) / 100.0;
        completion.text = "" + completion.text + tapsPercent + "%";

        trackedTime = trackedTime / 60f;
        double roundedTime = (Mathf.Round(trackedTime * 100)) / 100.0;
        time.text = "" + time.text + roundedTime + " minutes";
    }

    public void FixedUpdate()
    {
        if (!isGameFinished)
        {
            trackedTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            keyInRange = true;
            interactBox[0].SetActive(true);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            conInRange = true;
            interactBox[1].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            keyInRange = false;
            interactBox[0].SetActive(false);
        }
        if (other.CompareTag("PlayerController") && !other.isTrigger)
        {
            conInRange = false;
            interactBox[1].SetActive(false);
        }
    }
}
