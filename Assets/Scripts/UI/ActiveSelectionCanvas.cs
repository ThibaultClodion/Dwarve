using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelectionCanvas : MonoBehaviour
{
    private GameObject[] players;

    private void Awake()
    {
        //Find all the players
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            //Update the canvas
            players[i].GetComponent<Character>().ActiveSelectionCanvas();
            players[i].GetComponent<Ready>().ResetNbReady();
        }

    }
}
