using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Water : MonoBehaviour
{
    public string defaultURP = "Default Scriptable Render Pipeline Settings is URP-HighFidelity";
    public AudioSource underwaterSourceKey;
    public AudioSource underwaterSourceCon;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            player.StandUpright();

            player.isSwimming = true;
            player.ResetVelocity();

            if (player.gameObject.CompareTag("Player"))
            {
                GameObject.Find("UnderwaterVolumeKeyboard").GetComponent<Volume>().enabled = true;
                GameObject.Find("OverworldVolumeKeyboard").GetComponent<Volume>().enabled = false;
                GameObject.Find("MineVolumeKeyboard").GetComponent<Volume>().enabled = false;

                underwaterSourceKey.Play();
            }
            else
            {
                GameObject.Find("UnderwaterVolumeController").GetComponent<Volume>().enabled = true;
                GameObject.Find("OverworldVolumeController").GetComponent<Volume>().enabled = false;
                GameObject.Find("MineVolumeController").GetComponent<Volume>().enabled = false;

                underwaterSourceCon.Play();
            }
            //RenderSettings.fog = true; only enable when the cameras get different fog settings rendered
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            player.isSwimming = false;
            player.ResetVelocity();
            player.rb.AddForce(new Vector3(0, player.jumpForce, 0), ForceMode.Impulse);
            if (player.gameObject.CompareTag("Player"))
            {
                GameObject.Find("UnderwaterVolumeKeyboard").GetComponent<Volume>().enabled = false;
                GameObject.Find("OverworldVolumeKeyboard").GetComponent<Volume>().enabled = true;
                underwaterSourceKey.Stop();
            }
            else
            {
                GameObject.Find("UnderwaterVolumeController").GetComponent<Volume>().enabled = false;
                GameObject.Find("OverworldVolumeController").GetComponent<Volume>().enabled = true;

                underwaterSourceCon.Stop();
            }
            //RenderSettings.fog = false;
        }
    }
}
