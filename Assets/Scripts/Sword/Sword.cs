using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.XR;

public class Sword : MonoBehaviour
{
    #region Datas
    private HiltData hilt;
    private Transform[] slotsTransform;
    private int nbSlots;
    private GameObject actualHilt;     //Actual Prefab display

    private BladeData[] blades;
    [SerializeField] private BladeData emptyBlade;
    private int actualBladeIndex;
    

    [Header("Canvas Datas")]
    [SerializeField] private GameObject bladeButton;
    [SerializeField] private GameObject hiltButton;
    private GameObject parentCanvas;
    public GameObject[] buttons;

    [Header("Shop Datas")]
    [SerializeField] private GameObject bladeShop;
    [SerializeField] private GameObject hiltShop;
    private GameObject actualShop;
    private Character character;
    [NonSerialized] public bool isOnShop = false;
    [NonSerialized] public bool isSwitchingBlade = false;
    [NonSerialized] public int currentSwitchIndex;
    private int previousSwitchIndex;


    [Header("Reset Datas")]
    [SerializeField] private HiltData defaultHilt;
    [SerializeField] private BladeData[] defaultBlades;
    #endregion

    #region WeaponSpawn
    public void ResetSword()
    {
        hilt = defaultHilt;
        blades = defaultBlades;
    }

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
        if(hilt == null)
        {
            hilt = defaultHilt;
        }
        if(blades == null)
        {
            blades = defaultBlades;
        }

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
        //Initialisation of Datas
        int indexBlade = 0;
        int i = 0;
        buttons = new GameObject[nbSlots + 1];


        foreach (Transform t in actualHilt.transform) 
        {
            if(t.name.StartsWith("Hilt"))
            {
                GameObject hilt = t.gameObject;

                //Instantiate the hilt button
                GameObject button = Instantiate(hiltButton, bladeButton.transform.position, bladeButton.transform.rotation);
                button.transform.SetParent(hilt.transform, false);
                button.transform.Rotate(90, 0, 0); //Rotate because the hilt is at -90
                button.transform.localScale = new Vector3(0.007f, 0.04f, 1);

                //Add the buttons to the array
                buttons[i] = button;
                i++;

                //Set the hilt button for the character
                character.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(button);
            }
            else if(t.name.StartsWith("Blade"))
            {
                GameObject blade = t.gameObject;

                //Instantiate a blade button
                GameObject button = Instantiate(bladeButton, bladeButton.transform.position, bladeButton.transform.rotation);
                button.transform.SetParent(blade.transform, false);
                button.transform.localScale = new Vector3(0.008f, 0.06f, 1);

                //Add the buttons to the array
                buttons[i] = button;
                i++;

                //Give the index of blade
                button.GetComponent<SwordButton>().bladeIndex = indexBlade;
                indexBlade++;
            }
        }

        ButtonNavigationUpdate();
    }

    private void ButtonNavigationUpdate()
    {
        //Affect the good navigation between buttons

        //Find the buttons positions
        float[] xButtonPosition = new float[buttons.Length];
        float[] yButtonPosition = new float[buttons.Length];

        for(int i = 0; i < buttons.Length; i++) 
        {
            xButtonPosition[i] = buttons[i].transform.parent.position.x;
            yButtonPosition[i] = buttons[i].transform.parent.position.y;
        }

        float OnUpPos = yButtonPosition.Max();
        float OnLeftPos = xButtonPosition.Min();
        float OnDownPos = yButtonPosition.Min();
        float OnRightPos = xButtonPosition.Max();


        //Set selected on down, up, left or right
        for(int i = 0 ; i < buttons.Length ; i++) 
        {
            //Create a new navigation explicit
            Navigation NewNav = new Navigation();
            NewNav.mode = Navigation.Mode.Explicit;

            //If the button is not on the Top
            if (yButtonPosition[i] != OnUpPos)
            {
                float nearestUpDistance = 0f;
                float nearestHorizontalDistance = 10000f; //Sentry
                int bestIndex = 0;

                //Find the nearest up distance
                for (int j = 0 ; j < buttons.Length ; j++) 
                {
                    if (yButtonPosition[j] > yButtonPosition[i])
                    {
                        if(nearestUpDistance == 0f || Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) < nearestUpDistance) 
                        {
                            nearestUpDistance = Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]);
                        }
                    }
                }

                //Find the nearest horizontal button
                for(int j = 0;  j < buttons.Length ; j++) 
                {
                    if(Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) == nearestUpDistance)
                    {
                        if (Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) < nearestHorizontalDistance)
                        {
                            nearestHorizontalDistance = Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]);
                            bestIndex = j;
                        }
                    }
                }

                //Affect the good button
                NewNav.selectOnUp = buttons[bestIndex].GetComponent<Button>();
            }


            //If the button is not on the Bot
            if (yButtonPosition[i] != OnDownPos)
            {
                float nearestDownDistance = 0f;
                float nearestHorizontalDistance = 10000f; //Sentry
                int bestIndex = 0;

                //Find the nearest up distance
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (yButtonPosition[j] < yButtonPosition[i])
                    {
                        if (nearestDownDistance == 0f || Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) < nearestDownDistance)
                        {
                            nearestDownDistance = Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]);
                        }
                    }
                }

                //Find the nearest horizontal button
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) == nearestDownDistance)
                    {
                        if (Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) < nearestHorizontalDistance)
                        {
                            nearestHorizontalDistance = Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]);
                            bestIndex = j;
                        }
                    }
                }

                //Affect the good button
                NewNav.selectOnDown = buttons[bestIndex].GetComponent<Button>();
            }

            //If the button is not on the Left
            if (xButtonPosition[i] != OnLeftPos)
            {
                float nearestLeftDistance = 0f;
                float nearestVerticalDistance = 10000f; //Sentry
                int bestIndex = 0;

                //Find the nearest up distance
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (xButtonPosition[j] < xButtonPosition[i])
                    {
                        if (nearestLeftDistance == 0f || Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) < nearestLeftDistance)
                        {
                            nearestLeftDistance = Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]);
                        }
                    }
                }

                //Find the nearest vertical button
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) == nearestLeftDistance)
                    {
                        if (Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) < nearestVerticalDistance)
                        {
                            nearestVerticalDistance = Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]);
                            bestIndex = j;
                        }
                    }
                }

                //Affect the good button
                NewNav.selectOnLeft = buttons[bestIndex].GetComponent<Button>();
            }

            //If the button is not on the Right
            if (xButtonPosition[i] != OnRightPos)
            {
                float nearestRightDistance = 0f;
                float nearestVerticalDistance = 10000f; //Sentry
                int bestIndex = 0;

                //Find the nearest up distance
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (xButtonPosition[j] > xButtonPosition[i])
                    {
                        if (nearestRightDistance == 0f || Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) < nearestRightDistance)
                        {
                            nearestRightDistance = Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]);
                        }
                    }
                }

                //Find the nearest vertical button
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (Mathf.Abs(xButtonPosition[j] - xButtonPosition[i]) == nearestRightDistance)
                    {
                        if (Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]) < nearestVerticalDistance)
                        {
                            nearestVerticalDistance = Mathf.Abs(yButtonPosition[j] - yButtonPosition[i]);
                            bestIndex = j;
                        }
                    }
                }

                //Affect the good button
                NewNav.selectOnRight = buttons[bestIndex].GetComponent<Button>();
            }

            //Affect the navigation to the button
            buttons[i].GetComponent<Button>().navigation = NewNav;
        }
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

    #region Buttons

    public void SwitchBlade()
    {
        if(!isSwitchingBlade)
        {
            GameObject currentSelected = character.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject;

            if (currentSelected.name.Contains("Blade"))
            {
                isSwitchingBlade = true;
                currentSwitchIndex = GetButtonIndex(currentSelected);
                previousSwitchIndex = currentSwitchIndex;
            }
        }
    }

    public void SelectSwitch()
    {
        int selectIndex = GetButtonIndex(character.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject);

        //Return to Initial State
        SwitchTwoBlades(currentSwitchIndex, previousSwitchIndex);
        currentSwitchIndex = previousSwitchIndex;

        //Switch the two concern blades
        SwitchTwoBlades(currentSwitchIndex, selectIndex);
        currentSwitchIndex = selectIndex;

        //Select the good button and update the navigation
        StartCoroutine(selectButton(buttons[currentSwitchIndex]));
        ButtonNavigationUpdate();
    }

    private int GetButtonIndex(GameObject blade)
    {
        for(int i  = 0; i < buttons.Length; ++i) 
        {
            if (buttons[i] == blade)
            {
                return i;
            }
        }

        return -1;
    }

    private void SwitchTwoBlades(int index1, int index2)
    {
        //Be carreful that blades array has -1 because it doesn't count the hilt

        //Switch Blade Data
        BladeData temp = blades[index1-1];
        blades[index1-1] = blades[index2-1];
        blades[index2-1] = temp;

        //Switch button Data
        GameObject tempButton = buttons[index1];
        buttons[index1] = buttons[index2];
        buttons[index2] = tempButton;

        //Switch position
        GameObject blade1 = buttons[index1].transform.parent.gameObject;
        GameObject blade2 = buttons[index2].transform.parent.gameObject;

        Vector3 copyPos = blade1.transform.position - (blade1.transform.up * (blades[index1 - 1].bladeHeight / 2));
        Quaternion copyRot = blade1.transform.rotation;

        blade1.transform.SetPositionAndRotation(blade2.transform.position - (blade2.transform.up * (blades[index2 - 1].bladeHeight / 2)), blade2.transform.rotation);
        blade1.transform.position += blade1.transform.up * (blades[index1 - 1].bladeHeight / 2);

        blade2.transform.SetPositionAndRotation(copyPos, copyRot);
        blade2.transform.position += blade2.transform.up * (blades[index2 - 1].bladeHeight / 2);
    }

    public void ResetSwitchBlade()
    {
        isSwitchingBlade = false;

        //Return to Initial State
        SwitchTwoBlades(currentSwitchIndex, previousSwitchIndex);
        currentSwitchIndex = previousSwitchIndex;

        ButtonNavigationUpdate();
    }

    public void ConfirmSwitchBlade()
    {
        isSwitchingBlade = false;
    }

    IEnumerator selectButton(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        button.GetComponent<SwordButton>().canBeSelect = false;
        character.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(button);
    }

    public void OpenShop(int bladeIndex, bool isBladeShop)
    {
        //Update the blade index
        actualBladeIndex = bladeIndex;

        if(isBladeShop)
        {
            actualShop = Instantiate(bladeShop);
        }
        else
        {
            actualShop = Instantiate(hiltShop);
        }

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
            actualBladeIndex = 0;
        }
    }

    public void ChangeBlade(BladeData blade)
    {
        if(actualBladeIndex < blades.Length)
        {
            blades[actualBladeIndex] = blade;
        }
    }

    public void ChangeHilt(HiltData hilt)
    {
        this.hilt = hilt;
    }
    #endregion
}
