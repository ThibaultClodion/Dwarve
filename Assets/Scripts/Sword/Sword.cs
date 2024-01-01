using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;

public class Sword : MonoBehaviour
{
    #region Datas
    //Hilt Data
    [SerializeField] private HiltData hilt;
    private Transform[] slotsTransform;
    private int nbSlots;

    //Blades Data
    [SerializeField] private BladeData[] blades;
    [SerializeField] private BladeData emptyBlade;

    //Canvas Datas
    [SerializeField] private GameObject weaponButton;
    private GameObject parentCanvas;

    //Actual Prefab display
    private GameObject actualHilt;

    //Shop datas
    [SerializeField] private GameObject shopPrefab;
    private GameObject actualShop;
    private Character character;
    [NonSerialized] public bool isOnShop = false;
    #endregion

    #region WeaponSpawn
    // Initialize the Hilt and the Associate Blades In Game
    public void Init(GameObject hand)
    {
        UpdateCharacterDatas();
        UpdateWeaponData();
        DestroyOldWeapon();
        InstantiateWeapon(hand);
    }

    //Initialize the weapon in the modding menu
    public void InitModding(GameObject parent)
    {
        UpdateCharacterDatas();
        UpdateWeaponData();
        DestroyOldWeapon();
        InstantiateWeapon(parent);

        //Change the size and position of the sword
        actualHilt.transform.localScale = new Vector3(150, 1, 150);
        actualHilt.transform.Rotate(-90, 0, 0);

        CreateButtons();
    }

    private void UpdateCharacterDatas()
    {
        //Avoid some bugs
        character = GetComponent<Character>();
    }

    private void UpdateWeaponData()
    {
        Transform[] childrenTransform = hilt.hiltPrefab.GetComponentsInChildren<Transform>();

        //The two first are not slot so I ignore it
        nbSlots = childrenTransform.Length - 2;
        slotsTransform = new Transform[nbSlots];
        for (int i = 0; i < nbSlots; i++)
        {
            slotsTransform[i] = childrenTransform[i + 2];
        }

        //If there are less blade than slots, put the resting slot to empty
        BladeData[] newBlade = new BladeData[nbSlots];
        for (int i = 0; i < nbSlots; i++)
        {
            if (i < blades.Length)
            {
                newBlade[i] = blades[i];
            }
            else
            {
                newBlade[i] = emptyBlade;
            }
        }
        blades = newBlade;
    }

    public void DestroyOldWeapon()
    {
        //Destroy the old weapon
        if (actualHilt != null)
        {
            Destroy(actualHilt);
        }
    }

    private void CreateButtons()
    {
        GameObject mainButton = null;

        foreach (Transform t in actualHilt.transform) 
        {
            if(t.name.StartsWith("Hilt"))
            {
                GameObject hilt = t.gameObject;

                //Instantiate the hilt button
                GameObject hiltButton = Instantiate(weaponButton, weaponButton.transform.position, weaponButton.transform.rotation);
                hiltButton.transform.SetParent(hilt.transform, false);
                hiltButton.transform.Rotate(90, 0, 0); //Rotate because the hilt is at -90
                hiltButton.transform.localScale = new Vector3(0.007f, 0.04f, 1);

                mainButton = hiltButton;
            }
            else if(t.name.StartsWith("Blade"))
            {
                GameObject blade = t.gameObject;

                //Instantiate a blade button
                GameObject bladeButton = Instantiate(weaponButton, weaponButton.transform.position, weaponButton.transform.rotation);
                bladeButton.transform.SetParent(blade.transform, false);
                bladeButton.transform.localScale = new Vector3(0.008f, 0.06f, 1);
            }
        }

        //Set the main button for the character
        character.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(mainButton);
    }

    private void InstantiateWeapon(GameObject parent)
    {
        //update the parent canvas
        parentCanvas = parent;

        //Instantiate the hilt
        actualHilt = Instantiate(hilt.hiltPrefab, hilt.hiltPrefab.transform.position, hilt.hiltPrefab.transform.rotation);
        actualHilt.transform.SetParent(parent.transform, false);

        //Instantiate the blades
        for (int i = 0; i < nbSlots; i++)
        {
            GameObject blade = Instantiate(blades[i].bladePrefab, slotsTransform[i].position, slotsTransform[i].rotation);

            //Move the blade following the green axis (be careful about that when setting the slot) depending on the it height.
            blade.transform.position += blade.transform.up * (blades[i].bladeHeight / 2);

            blade.transform.SetParent(actualHilt.transform, false);
        }
    }
    #endregion

    #region Shop
    public void OpenShop()
    {
        actualShop = Instantiate(shopPrefab);
        actualShop.transform.SetParent(parentCanvas.transform, false);

        //Destroy the old weapon
        DestroyOldWeapon();

        //Set the selected button of the character
        character.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(actualShop);
        isOnShop = true;
    }

    public void CloseShop()
    {
        //Destroy the shop and visualize again the sword
        if (actualShop != null)
        {
            Destroy(actualShop);
            InitModding(parentCanvas);
            isOnShop = false;
        }
    }
    #endregion
}
