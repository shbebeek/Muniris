using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GateSwitch : MonoBehaviour
{
    public GameObject lever;
    public GameObject gate;
    public AudioSource leverSound;
    public AudioSource gateSound;

    public bool isOpen;
    public BoolValue storedOpen;

    public GameObject[] interactBox; // keyboard before player

    public Animator anim;
    public AnimationClip switchClip;
    public AnimationClip switchClose;

    public Animator gateAnim;
    public AnimationClip gateOpenClip;
    public AnimationClip gateClose;

    public bool canUse = true;

    public PlayerMovement key;
    public PlayerMovement con;
    public bool keyInRange;
    public bool conInRange;

    void Start()
    {
        isOpen = storedOpen.RuntimeValue;
        canUse = !storedOpen.RuntimeValue;

        if (isOpen)
        {
            anim.Play(switchClip.name);
            gateAnim.Play(gateOpenClip.name);
        }

        key = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        con = GameObject.FindWithTag("PlayerController").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (((key.playerInput.OnFoot.PickUp.WasPressedThisFrame() && keyInRange) || (con.playerControllerInput.GroundTraversal.PickUp.WasPressedThisFrame() && conInRange)) && canUse)
        {
            if (!isOpen)
            {
                StartCoroutine(OpenSwitch());
            }
            else
            {
                StartCoroutine(CloseSwitch());
            }
        }
    }

    public IEnumerator OpenSwitch()
    {
        isOpen = true;
        storedOpen.RuntimeValue = isOpen;
        anim.Play(switchClip.name);
        gateAnim.Play(gateOpenClip.name);

        GameObject.Find("LeverGrind").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        GameObject.Find("GateOpen").GetComponent<AudioSource>().Play();
    }

    public IEnumerator CloseSwitch()
    {
        isOpen = false;
        storedOpen.RuntimeValue = isOpen;
        anim.Play(switchClose.name);
        gateAnim.Play(gateClose.name);

        GameObject.Find("LeverGrind").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        GameObject.Find("GateOpen").GetComponent<AudioSource>().Play();
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
