using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;
    [SerializeField] private GameObject[] players;
    [SerializeField] private TargetGroupAutomatic targetGroupAutomatic;


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        //Find all the players
        players = GameObject.FindGameObjectsWithTag("Player");

        //Change they're location
        for(int i = 0; i < players.Length; i++) 
        {
            players[i].transform.position = spawns[i].position;
        }

        //Update the camera
        targetGroupAutomatic.UpdateTarget();

        //Wait 1 Seconds Before Playing
        //After I would make a countdown before a wave begin
        StartCoroutine(WaitBeforePlay());
    }

    IEnumerator WaitBeforePlay()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Character>().isPlaying = true;
        }
    }

    void Update()
    {

    }
}
