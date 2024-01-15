using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    #region Datas
    private GameManager gameManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject player;
    private PlayerInput input;

    //Datas
    [NonSerialized] public bool isReadyForNextScene = false;
    [NonSerialized] public bool isOnGame = false;

    //Character Data
    [SerializeField] private Sword sword;
    private int money = 5;
    #endregion

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        input = GetComponent<PlayerInput>();
    }

    #region Input
    public void OnDeviceLost()
    {
        //Destroy the character if he left the game (ex: the controller has no batteries)
        gameManager.CharacterLeft(this);
        Destroy(gameObject);
    }

    public void OnCancel()
    {
        if(!isReadyForNextScene && !(SceneManager.GetActiveScene().name == "WeaponModding"))
        {
            gameManager.ChangeToPreviousScene(this);
        }
        else if(SceneManager.GetActiveScene().name == "WeaponModding" && sword.isOnShop)
        {
            sword.CloseShop();
        }           
        else if(SceneManager.GetActiveScene().name == "WeaponModding" && sword.isSwitchingBlade)
        {
            sword.ResetSwitchBlade();
        }        
        else if(SceneManager.GetActiveScene().name == "WeaponModding" && !isReadyForNextScene)
        {
            gameManager.WeaponIsReady(this);
        }        
        else if(SceneManager.GetActiveScene().name == "WeaponModding" && isReadyForNextScene)
        {
            gameManager.WeaponIsNotReady(this);
        }
        else
        {
            gameManager.CharacterIsNotReady(this);
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

    public void OnNorthButton()
    {
        //Make the sword on switching blades state
        if (!isOnGame)
        {
            sword.SwitchBlade();
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
    #endregion

    #region PlayerManager
    public void InitPlayer(Vector3 pos)
    {
        player.SetActive(true);
        player.transform.position = pos;

        //Give the player controller information that the character link is this one
        playerController.character = this;
        //Init the weapon
        sword.Init(playerController.hand);
        //Reset player datas
        playerController.ResetData();

        //Wait before being able to move etc (This avoid strange bug also like a reset of position)
        //After make the time of waiting link to the gameManager countdown
        StartCoroutine(WaitBeforePlay());
    }

    IEnumerator WaitBeforePlay()
    {
        yield return new WaitForSeconds(1);

        isOnGame = true;
    }

    public void Die()
    {
        DisablePlayer();
        gameManager.PlayerDie(this);
    }

    public void DisablePlayer()
    {
        isOnGame = false;
        player.SetActive(false);
    }

    public void ResetDatas()
    {
        //Reset the data to start a new game
        money = 5;
        sword.ResetSword();
    }
    #endregion

    #region SwordManagement
    public void InitWeaponModding(GameObject parent)
    {
        //Make here or there are some bugs
        sword = GetComponent<Sword>();

        sword.InitModding(parent);
    }

    public bool MoneyExchange(int amount)
    {
        //Exchange amount of money and return if the transaction succeed
        if(amount > 0)
        {
            money += amount;
            return true;
        }
        else
        {
            if(money < -amount)
            {
                return false;
            }
            else
            {
                money += amount;
                return true;
            }
        }
    }

    public int GetMoney() { return money; } 
    public Sword GetSword() { return sword; }
    #endregion
}
