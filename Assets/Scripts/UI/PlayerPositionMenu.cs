using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerPositionMenu : MonoBehaviour
{
    private Vector3 onePlayerPos;
    private Vector3[] twoPlayerPos;
    private Vector3[] threePlayerPos;
    private Vector3[] fourPlayerPos;

    private void Awake()
    {
        onePlayerPos = new Vector3(0, 0, 0);
        twoPlayerPos = new Vector3[2] { new Vector3(-5, 0, 0), new Vector3(5, 0, 0) };

        //Il faudra connecter 3 à 4 manettes pour faire le test
        threePlayerPos = new Vector3[3] { new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(0,0,0)};
        fourPlayerPos = new Vector3[4] { new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(0,0,0), new Vector3(0,0,0)};
    }

    public void UpdatePlayerPos()
    {
        //Find all the players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if(players.Length == 0)
        {
            return;
        }
        else if(players.Length == 1) 
        {
                players[0].transform.position = onePlayerPos;
        }
        else if(players.Length == 2) 
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position = twoPlayerPos[i];
            }
        }
        else if (players.Length == 3)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position = threePlayerPos[i];
            }
        }
        else if (players.Length == 4)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position = fourPlayerPos[i];
            }
        }
    }
}
