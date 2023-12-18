using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Character : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void OnDeviceLost()
    {
        //Destroy the player if he left the game (ex: the controller has no batteries)
        gameManager.CharacterLeft(this);
        Destroy(gameObject);
    }
}
