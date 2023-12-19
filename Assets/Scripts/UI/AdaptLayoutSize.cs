using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptLayoutSize : MonoBehaviour
{

    private GridLayoutGroup layoutGroup;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        layoutGroup = GetComponent<GridLayoutGroup>();

        if(players.Length >= 3)
        {
            layoutGroup.cellSize = new Vector2(586, 329.5f);
        }
        else
        {
            layoutGroup.cellSize = new Vector2(586, 659);
        }
    }
}
