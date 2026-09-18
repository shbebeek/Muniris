using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject hitUI;
    public GameObject deathUI;
    public TextMeshProUGUI ammoText;

    public RectTransform[] healthSliders;
    public RectTransform healthBackground;
    public Image healthBar;
    public Gradient healthGradient;

    public RectTransform[] staminaSliders;
    public RectTransform staminaBackground;
    public Image staminaBar;
    public Gradient staminaGradient;

    public Image magicBar;
    public Gradient magicGradient;
    public Inventory inv;

    public void Start()
    {
        ResizeHealth();
        ResizeStamina();
    }

    public void ResizeHealth()
    {
        foreach (RectTransform slider in healthSliders)
        {
            float org_width = inv.maxHealth * 3f;
            slider.sizeDelta = new Vector2(org_width, slider.sizeDelta.y);
            healthBackground.sizeDelta = new Vector2(org_width + 15, healthBackground.sizeDelta.y);
        }
    }

    public void ResizeStamina()
    {
        foreach (RectTransform slider in staminaSliders)
        {
            float org_width = inv.maxStamina * 3f;
            slider.sizeDelta = new Vector2(org_width, slider.sizeDelta.y);
            staminaBackground.sizeDelta = new Vector2(org_width + 15, staminaBackground.sizeDelta.y);
        }
    }

    private void Awake()
    {
        instance = this;
        deathUI.SetActive(false);
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI,transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        deathUI.SetActive(false);

        if (CompareTag("KeyboardUI"))
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = player.GetComponent<PlayerMovement>().respawnPoint.position;
            player.GetComponent<PlayerHealth>().Respawn();
        }
        else
        {
            GameObject player = GameObject.FindWithTag("PlayerController");
            player.transform.position = player.GetComponent<PlayerMovement>().respawnPoint.position;
            player.GetComponent<PlayerHealth>().Respawn();
        }
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
    }

    public void SetHealthValue(int health)
    {
        float floatHealth = (float) health / inv.maxHealth;
        healthBar.color = healthGradient.Evaluate(floatHealth);
        healthBar.fillAmount = floatHealth;
    }

    public void SetStaminaValue(double stamina)
    {
        float floatStamina = (float) stamina / inv.maxStamina;
        staminaBar.color = staminaGradient.Evaluate(floatStamina);
        staminaBar.fillAmount = floatStamina;
    }

    public void SetMagicValue(int magic)
    {
        float floatMagic = (float) magic / inv.maxMagic;
        magicBar.color = magicGradient.Evaluate(floatMagic);
        magicBar.fillAmount = floatMagic;
    }

    public void Update()
    {
        SetMagicValue((int) inv.currentMagic);
    }
}
