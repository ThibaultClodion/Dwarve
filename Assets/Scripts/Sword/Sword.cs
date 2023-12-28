using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Sword : MonoBehaviour
{
    //Hilt Data
    [SerializeField] private HiltData hilt;
    private Transform[] slotsTransform;
    private int nbSlots;

    //Blades Data
    [SerializeField] private BladeData[] blades;
    [SerializeField] private BladeData emptyBlade;

    //Canvas Datas
    [SerializeField] private GameObject weaponButton;

    //Actual Prefab display
    private GameObject actualHilt;

    // Initialize the Hilt and the Associate Blades In Game
    public void Init(GameObject hand)
    {
        UpdateWeaponData();
        DestroyOldWeapon();
        InstantiateWeapon(hand);
    }

    //Initialize the weapon in the modding menu
    public GameObject InitModding(GameObject parent)
    {
        UpdateWeaponData();
        DestroyOldWeapon();
        InstantiateWeapon(parent);

        //Change the size and position of the sword
        actualHilt.transform.localScale = new Vector3(150, 1, 150);
        actualHilt.transform.Rotate(-90, 0, 0);

        return CreateButtons();
    }

    private GameObject CreateButtons()
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

        return mainButton;
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

    private void InstantiateWeapon(GameObject parent)
    {
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
}
