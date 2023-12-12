using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;
    public GameObject[] players;
    private Character[] characters;
    private int[] nbWins;
    [SerializeField] private TargetGroupAutomatic targetGroupAutomatic;

    //Game parameters
    [SerializeField] private int nbRoundToWin;
    public int nbPlayersAlive;


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        //Find all the players
        players = GameObject.FindGameObjectsWithTag("Player");

        //Initialize the arrrays
        characters = new Character[players.Length];
        nbWins = new int[players.Length];
        nbPlayersAlive = players.Length;

        for(int i = 0; i < players.Length; i++) 
        {

            //Get the characters script and make them get the GameManagerScript
            characters[i] = players[i].GetComponent<Character>();
            characters[i].getGameManager();

            //Initialize the nbWins
            nbWins[i] = 0;     
        }

        StartRound();

    }

    public void StartRound()
    {
        nbPlayersAlive = players.Length;

        for (int i = 0; i < players.Length; i++)
        {
            //All the player stop moving
            characters[i].isPlaying = false;

            //Change player location and active them
            players[i].transform.position = spawns[i].position;
        }

        //Allow to wait before new Round and make countdown
        StartCoroutine(WaitBeforePlay());
    }

    IEnumerator WaitBeforePlay()
    {
        //Wait a bit before active all players for don't having bugs
        yield return new WaitForSeconds(0.01f);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(true);
        }

        //Update the camera
        targetGroupAutomatic.UpdateTarget();

        //Countdown before allow players to fight
        yield return new WaitForSeconds(1);

        for (int i = 0; i < players.Length; i++)
        {
            characters[i].isPlaying = true;
        }
    }

    public void Victory()
    {
        //Find the character that is alive (so that's still playing)
        int winnerIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (characters[i].isPlaying == true)
            {
                winnerIndex = i;
            }
        }

        nbWins[winnerIndex]++;

        //Launch a new round if the game isn't finish
        if (nbWins[winnerIndex] == nbRoundToWin)
        {
            Debug.Log(players[winnerIndex] + "Has win the game");
        }
        else
        {
            StartRound();
        }
    }
}
