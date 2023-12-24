using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdaptLayoutSize : MonoBehaviour
{
    private GridLayoutGroup layoutGroup;

    public void UpdateSize()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        layoutGroup = GetComponent<GridLayoutGroup>();

        //Canvas settings
        float width = 1172;
        float height = 659;

        if(SceneManager.GetActiveScene().name != "VictoryScene")
        {
            if (players.Length >= 3)
            {
                layoutGroup.cellSize = new Vector2(width/2, height/2);
            }
            else
            {
                layoutGroup.cellSize = new Vector2(width/2, height);
            }
        }
        else
        {
            if(players.Length == 1 )
            {
                layoutGroup.cellSize = new Vector2(width, 100);
            }
            else if (players.Length == 2)
            {
                layoutGroup.cellSize = new Vector2(width / 2, 100);
            }
            else if (players.Length == 3)
            {
                layoutGroup.cellSize = new Vector2(width / 3, 100);
            }
            else
            {
                layoutGroup.cellSize = new Vector2(width / 4, 100);
            }
        }
    }
}
