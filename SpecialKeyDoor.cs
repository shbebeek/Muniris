using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialKeyDoor : LockedDoor
{
    public BoolValue hasKey;

    public override void Update()
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
                if (hasKey.RuntimeValue == true)
                {
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

    public override IEnumerator SayLocked()
    {
        foreach (TextMeshProUGUI dialog in dialogText)
        {
            dialog.text = "Sealed forever.";
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
}
