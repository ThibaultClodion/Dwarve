using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelection : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Material[] materials;
    public int materialIndex = 0;

    public void IncrementIndex()
    {
        materialIndex++;
        UpdateColor();
    }

    public void DecrementIndex()
    {
        materialIndex--;
        UpdateColor();
    }

    public void UpdateColor()
    {
        player.GetComponent<MeshRenderer>().material = materials[Mathf.Abs(materialIndex % materials.Length)];
    }
}
