using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatUpgrade : MonoBehaviour
{
    public Inventory playerInventory;
    public string statDescription;
    public bool isHealth;
    public bool isStamina;
    public bool isMagic;
    public int weight;
    public GameObject key;
    public GameObject con;
    public GameObject keyui;
    public GameObject conui;

    public virtual void ApplyUpgrade()
    {
        key = GameObject.FindWithTag("Player");
        con = GameObject.FindWithTag("PlayerController");

        keyui = GameObject.FindWithTag("KeyboardUI");
        conui = GameObject.FindWithTag("ControllerUI");

        if (isHealth)
        {
            // numbers
            playerInventory.maxHealth += weight;
            key.GetComponent<PlayerHealth>().health = playerInventory.maxHealth;
            con.GetComponent<PlayerHealth>().health = playerInventory.maxHealth;
            statDescription = "You got a health upgrade! Your max health capacity is now increased.";

            // ui
            keyui.GetComponent<UIManager>().ResizeHealth();
            conui.GetComponent<UIManager>().ResizeHealth();

            keyui.GetComponent<UIManager>().SetHealthValue(playerInventory.maxHealth);
            conui.GetComponent<UIManager>().SetHealthValue(playerInventory.maxHealth);
        }
        else if (isStamina)
        {
            playerInventory.maxStamina += weight;
            key.GetComponent<PlayerMovement>().stamina = playerInventory.maxStamina;
            con.GetComponent<PlayerMovement>().stamina = playerInventory.maxStamina;
            statDescription = "You got a stamina upgrade! You can now exert more energy without getting tired.";

            keyui.GetComponent<UIManager>().ResizeStamina();
            conui.GetComponent<UIManager>().ResizeStamina();

            keyui.GetComponent<UIManager>().SetStaminaValue(playerInventory.maxStamina);
            conui.GetComponent<UIManager>().SetStaminaValue(playerInventory.maxStamina);
        }
        else if (isMagic)
        {
            playerInventory.maxMagic += weight;
            playerInventory.currentMagic = playerInventory.maxMagic;
            statDescription = "You got a magic upgrade! Left-hand items now consume less magic upon use.";
        }
    }
}
