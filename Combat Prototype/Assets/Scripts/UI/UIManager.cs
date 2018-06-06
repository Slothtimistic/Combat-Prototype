using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager UI;

    public GameObject Inventory;

    public Image energyImage, healthImage;

    private void Awake()
    {
        UI = this;

        Player.UpdateUI UpdateUI = (e, state) =>
        {
            if (e == "EVENT_OPEN_INVENTORY")
            {
                Inventory.SetActive(!Inventory.activeSelf);
            }

            if(Inventory.activeSelf)
            {
                //unlock mouse
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                //lock mouse
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Debug.Log(e);
        };

        Player.OnUIChange += UpdateUI;
    }

    public static void UpdatePlayerUI(float currentEnergy, float maxEnergy, float currentHealth, float maxHealth)
    {
        UI.energyImage.rectTransform.offsetMax = new Vector2(-500 + (currentEnergy * (500 / maxEnergy)), 0);    //was 2.5f
        UI.healthImage.rectTransform.offsetMax = new Vector2(-500 + (currentHealth * (500 / maxHealth)), 0);    //was 100
    }

}
