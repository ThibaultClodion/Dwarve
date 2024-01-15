using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class HiltData : ScriptableObject
{
    //Hilt Data
    public GameObject hiltPrefab;
    public float length;
    public float attackCooldown;
    public AnimationClip attack;

    //Shop datas
    public int price;
    public string nameDisplay;
    public Sprite sprite;
}
