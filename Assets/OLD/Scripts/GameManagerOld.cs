using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerOld : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;
    public GameObject[] players;
    private CharacterOld[] characters;
    private int[] nbWins;
    [SerializeField] private TargetGroupAutomatic targetGroupAutomatic;

    //Game parameters
    [SerializeField] private int nbRoundToWin;
    public int nbPlayersAlive;  //The characters need to acces this data

    //Canvas
    [SerializeField] private GameObject countdownGO;
    [SerializeField] private TextMeshProUGUI countdownText;    
    [SerializeField] private GameObject winningGO;
    [SerializeField] private TextMeshProUGUI winningText; 


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        //Find all the players
        players = GameObject.FindGameObjectsWithTag("Player");

        //Initialize the arrrays and parameters
        characters = new CharacterOld[players.Length];
        nbWins = new int[players.Length];
        nbPlayersAlive = players.Length;

        for (int i = 0; i < players.Length; i++) 
        {

            //Get the characters script and make them get the GameManagerScript
            characters[i] = players[i].GetComponent<CharacterOld>();
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

            //Change player location
            players[i].transform.position = spawns[i].position;
        }

        //Allow to wait before new Round and make countdown
        StartCoroutine(WaitBeforePlay());
    }

    IEnumerator WaitBeforePlay()
    {
        countdownGO.SetActive(true);
        countdownText.text = "3";

        //Wait a bit before active all players for don't having bugs
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = spawns[i].position;
            players[i].SetActive(true);
        }

        //Update the camera
        targetGroupAutomatic.UpdateTarget();


        //Countdown
        yield return new WaitForSeconds(1);
        countdownText.text = "2";        
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        for (int i = 0; i < players.Length; i++)
        {
            characters[i].isPlaying = true;
        }

        countdownGO.SetActive(false);
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

        //Detect if the game is finish or not
        if (nbWins[winnerIndex] == nbRoundToWin)
        {
            winningGO.SetActive(true);
            winningText.text = "Player number " + (winnerIndex + 1) + "win !!";
        }

        //Creation of the weapons
        else
        {
            //Reactive the players and update they're ready script
            for (int i = 0; i < players.Length; i++)
            {
                players[i].SetActive(true);
            }

            //Open the weapon modding scene
            SceneManager.LoadScene("WeaponSelection");
        }
    }
}
