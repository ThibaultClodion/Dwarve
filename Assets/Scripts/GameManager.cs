using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;
    [SerializeField] private GameObject[] players;
    [SerializeField] private TargetGroupAutomatic targetGroupAutomatic;

    public bool isPlaying = false;

    void Start()
    {
        //Force to wait one second or the player is falling infinitly
        //It's the only way i find to resolve the problem
        StartCoroutine(WaitForPlaying(1));
    }

    IEnumerator WaitForPlaying(int  seconds)
    {
        yield return new WaitForSeconds(seconds);
        isPlaying = true;
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
    }

    void Update()
    {
        if (!isPlaying) 
        {
            StartGame();
        }
    }
}
