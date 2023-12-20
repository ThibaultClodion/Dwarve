using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject[] disableObjects; 
    [SerializeField] private string nextScene; 

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void IsReady()
    {
        gameManager.PlayerIsReady(int.Parse(gameObject.name) - 1, disableObjects, nextScene);
    }
}
