using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MinePostProcessing : MonoBehaviour
{
    public bool isInMine = false;

    public Volume mineVolume;
    public Volume overworldVolume;

    public Volume mineVolumeController;
    public Volume overworldVolumeController;

    public float transitionTime = 2.0f;

    public Coroutine activeRoutine;
    public Coroutine activeRoutineController;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            if (player.gameObject.CompareTag("Player"))
            {
                FadeVolumeKeyboard(true);
            }
            else
            {
                FadeVolumeController(true);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "EyeLevel")
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            if (player.gameObject.CompareTag("Player"))
            {
                FadeVolumeKeyboard(false);
            }
            else
            {
                FadeVolumeController(false);
            }
        }
    }

    public void FadeVolumeKeyboard(bool targetMine)
    {
        // stop currently running fade
        if (activeRoutine != null) {
            StopCoroutine(activeRoutine);
        }

        // start new fade
        activeRoutine = StartCoroutine(FadeRoutine(targetMine, false));
    }

    public void FadeVolumeController(bool targetMine)
    {
        // stop currently running fade
        if (activeRoutineController != null)
        {
            StopCoroutine(activeRoutineController);
        }

        // start new fade
        activeRoutineController = StartCoroutine(FadeRoutine(targetMine, true));
    }

    public IEnumerator FadeRoutine(bool targetMine, bool isController)
    {
        float startWeightM;
        float startWeightO;

        float targetWeightM;
        float targetWeightO;

        if (isController)
        {
            startWeightM = mineVolumeController.weight;
            startWeightO = overworldVolumeController.weight;

            targetWeightM = targetMine ? 1.0f : 0.0f;
            targetWeightO = targetMine ? 0.0f : 1.0f;
        }
        else
        {
            startWeightM = mineVolume.weight;
            startWeightO = overworldVolume.weight;

            targetWeightM = targetMine ? 1.0f : 0.0f;
            targetWeightO = targetMine ? 0.0f : 1.0f;
        }

        float elapsedTime = 0f;

        while(elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionTime;

            if (isController)
            {
                mineVolumeController.weight = Mathf.Lerp(startWeightM, targetWeightM, normalizedTime);
                overworldVolumeController.weight = Mathf.Lerp(startWeightO, targetWeightO, normalizedTime);
            }
            else
            {
                mineVolume.weight = Mathf.Lerp(startWeightM, targetWeightM, normalizedTime);
                overworldVolume.weight = Mathf.Lerp(startWeightO, targetWeightO, normalizedTime);
            }

            yield return null;
        }

        if (isController)
        {
            mineVolumeController.weight = targetWeightM;
            overworldVolumeController.weight = targetWeightO;
        }
        else
        {
            mineVolume.weight = targetWeightM;
            overworldVolume.weight = targetWeightO;
        }

        activeRoutine = null;
    }
}
