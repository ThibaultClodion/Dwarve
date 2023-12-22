using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject player;
    private PlayerInput input;

    //Datas
    [NonSerialized] public bool isReadyForNextScene = false;
    [NonSerialized] public bool isOnGame = false;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        input = GetComponent<PlayerInput>();
    }

    public void OnDeviceLost()
    {
        //Destroy the player if he left the game (ex: the controller has no batteries)
        gameManager.CharacterLeft(this);
        Destroy(gameObject);
    }

    public void OnCancel()
    {
        if(!isReadyForNextScene)
        {
            gameManager.ChangeToPreviousScene(this);
        }
        else
        {
            gameManager.PlayerIsNotReady(this);
        }
    }

    public void Update()
    {
        //If the character isOnGame then move the player
        if(isOnGame)
        {
            playerController.Movement();
        }
    }

    //Update the player's movement
    public void OnMove(InputValue pos)
    {
        if(isOnGame)
        {
            playerController.ChangeMove(pos);
        }
    }
        
    //Update the player's rotation
    public void OnRotate(InputValue pos)
    {
        bool useKeyboard = (input.currentControlScheme == "Keyboard&Mouse");
        if (isOnGame)
        {
            playerController.ChangeRotate(pos, useKeyboard);
        }
    }

    //Make the player attack
    public void OnFire()
    {
        if (isOnGame)
        {
            playerController.Attack();
        }
    }

    //Make the player dash
    public void OnDash()
    {
        if (isOnGame)
        {
            playerController.Dash();
        }
    }



    public void InitPlayer(Vector3 pos)
    {
        player.SetActive(true);
        player.transform.position = pos;

        //Wait before being able to move etc (This avoid strange bug also like a reset of position)
        //After make the time of waiting link to the gameManager countdown
        StartCoroutine(WaitBeforePlay());
    }

    IEnumerator WaitBeforePlay()
    {
        yield return new WaitForSeconds(1);
        isOnGame = true;
    }
}
