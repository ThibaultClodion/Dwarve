using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HiltData : ScriptableObject
{
    //Hilt Data
    [SerializeField] private GameObject hiltPrefab;
    private Transform[] slotsTransform;
    private int nbSlots;

    //Blades Data
    [SerializeField] private BladeData[] blades;
    [SerializeField] private BladeData emptyBlade;

    // Initialize the Hilt and the Associate Blades
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
}
