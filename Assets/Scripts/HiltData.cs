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

    // Initialize the blade and the Hilt
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

        //Instantiate the hilt
        GameObject hilt = Instantiate(hiltPrefab, hiltPrefab.transform.position, hiltPrefab.transform.rotation);
        hilt.transform.SetParent(hand.transform, false);

        //Instantiate the blades
        for(int i = 0; i < nbSlots; i++) 
        {
            GameObject blade = Instantiate(blades[i].bladePrefab, slotsTransform[i].position, slotsTransform[i].rotation);
            blade.transform.SetParent(hilt.transform, false);
        }
        
    }
}
