using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BladeData : ScriptableObject
{
    //Game Datas
    [SerializeField] public GameObject bladePrefab;
    [SerializeField] public float bladeHeight;

    //Shop Datas
    public int price;
    public Sprite sprite;
    public string nameDisplay;
}
