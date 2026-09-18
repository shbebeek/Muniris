using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlower : LeftHandItem
{
    public float timeSlow = 0.5f;
    public bool timeSlowed = false;
    public GameObject bluePhase;
    public Animator hoderAnim;

    public bool isSyphoning = false;

    public override void Start()
    {
        bluePhase = GameObject.Find("BluePhase");
        bluePhase.GetComponent<Image>().enabled = false;
        bluePhase.SetActive(true);
    }

    public override void UseItem(UIManager user)
    {
        ui = user;
        timeSlowed = !timeSlowed;
        if (timeSlowed)
        {
            if (bluePhase.GetComponent<Image>().enabled)
            {
                timeSlowed = false;
            }
            else
            {
                Time.timeScale = timeSlow;
                anim.enabled = true;
                bluePhase.GetComponent<Image>().enabled = true;
                hoderAnim.enabled = true;
            }
        }
        else
        {
            Time.timeScale = 1.0f;
            anim.enabled = false;
            bluePhase.GetComponent<Image>().enabled = false;
            hoderAnim.enabled = false;
        }
    }

    public void Update()
    {
        if (!isSyphoning)
        {
            StartCoroutine(SyphonMagic());
        }
    }

    public IEnumerator SyphonMagic()
    {
        isSyphoning = true;
        if (timeSlowed)
        {
            if(playerInventory.currentMagic >= magicCost)
            {
                playerInventory.currentMagic -= magicCost;
                ui.SetMagicValue((int) playerInventory.currentMagic);
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                UseItem(ui);
            }
        }
        else
        {
            
        }
        isSyphoning = false;
    }
}
