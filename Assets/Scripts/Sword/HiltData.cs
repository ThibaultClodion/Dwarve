using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class HiltData : ScriptableObject
{
    //Hilt Data
    public GameObject hiltPrefab;
    public Transform[] slotsTransform;
    public int nbSlots;

    //Blades Data
    public BladeData[] blades;
    public BladeData emptyBlade;

    // Initialize the Hilt and the Associate Blades In Game
    public void Init(GameObject hand)
    {
        Transform[] childrenTransform = hiltPrefab.GetComponentsInChildren<Transform>();

        //The two first are not slot so I ignore it
        nbSlots = childrenTransform.Length-2;
        slotsTransform = new Transform[nbSlots];
        for(int i = 0; i < nbSlots; i++)
        {
            slotsTransform[i] = childrenTransform[i + 2];
        }

        //If there are less blade than slots, put the resting slot to empty
        BladeData[] newBlade = new BladeData[nbSlots];
        for(int i = 0; i < nbSlots; i++)
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

        //Instantiate the hilt
        GameObject hilt = Instantiate(hiltPrefab, hiltPrefab.transform.position, hiltPrefab.transform.rotation);
        hilt.transform.SetParent(hand.transform, false);

        //Instantiate the blades
        for(int i = 0; i < nbSlots; i++) 
        {
            GameObject blade = Instantiate(blades[i].bladePrefab, slotsTransform[i].position, slotsTransform[i].rotation);

            //Move the blade following the green axis (be careful about that when setting the slot) depending on the it height.
            blade.transform.position += blade.transform.up * (blades[i].bladeHeight / 2);

            blade.transform.SetParent(hilt.transform, false);
        }    
    }

    //Initialize the weapon in the modding menu
    public void InitModding(GameObject parent)
    {
        //Instantiate the hilt
        GameObject hilt = Instantiate(hiltPrefab, hiltPrefab.transform.position, hiltPrefab.transform.rotation);
        hilt.transform.SetParent(parent.transform, false);

        //Instantiate the blades
        for (int i = 0; i < nbSlots; i++)
        {
            GameObject blade = Instantiate(blades[i].bladePrefab, slotsTransform[i].position, slotsTransform[i].rotation);

            //Move the blade following the green axis (be careful about that when setting the slot) depending on the it height.
            blade.transform.position += blade.transform.up * (blades[i].bladeHeight / 2);

            blade.transform.SetParent(hilt.transform, false);
        }

        //Change the size and position
        hilt.transform.localScale = new Vector3(150, 1, 150);
        hilt.transform.Rotate(-90, 0, 0);
    }
}
