using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class HiltData : ScriptableObject
{
    //Hilt Data
    public GameObject hiltPrefab;
    public int price;
    public string nameDisplay;
    public Sprite sprite;
    public float length;

    public AnimationClip attack;

}
