using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HiltData : ScriptableObject
{
    //Hilt Data
    [SerializeField] private GameObject hiltPrefab;

    //Blade Slot Data
    [SerializeField] private int nbSlots;
    [SerializeField] private Vector3[] slotsPosition;
    [SerializeField] private Vector3[] slotsRotation;
    [SerializeField] private BladeData[] blades;

    // Initialize the blade and the Hilt
    public void Init(GameObject hand)
    {
        GameObject hilt = Instantiate(hiltPrefab, hiltPrefab.transform.position, hiltPrefab.transform.rotation);
        hilt.transform.SetParent(hand.transform, false);
        
        for(int i = 0; i < nbSlots; i++) 
        {
            Quaternion rotation = Quaternion.Euler(slotsRotation[i]);
            GameObject blade = Instantiate(blades[i].bladePrefab, slotsPosition[i] + new Vector3(0,0,blades[i].bladeHeight / 2), rotation);

            blade.transform.SetParent(hand.transform, false);
        }
    }
}
