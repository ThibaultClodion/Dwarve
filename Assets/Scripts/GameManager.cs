using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Characters Data
    private Character[] characters = new Character[4];
    private int nbPlayers = 0;

    //One player canvas Menu
    private bool onOnePlayerMenu;
    private GameObject onePlayerButton;

    //Scene Memory
    private string actualScene;
    private string previousScene;

    private void Start()
    {
        //Start is consider as Changing Scene
        actualScene = SceneManager.GetActiveScene().name;
        previousScene = SceneManager.GetActiveScene().name;
        ChangeScene();
    }

    #region ScenesManagement
    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneWait());
    }

    IEnumerator ChangeSceneWait()
    {
        //Wait a bit to let the scene load
        yield return new WaitForSeconds(0.1f);

        //Update datas
        UpdateSceneBool();
        previousScene = actualScene;
        actualScene = SceneManager.GetActiveScene().name;

        if (onOnePlayerMenu)
        {
            onePlayerButton = GameObject.FindGameObjectWithTag("MainButton");

            if (nbPlayers > 0)
            {
                characters[0].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
            }
        }
    }

    public void UpdateSceneBool()
    {
        onOnePlayerMenu = SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "SettingsMenu" || SceneManager.GetActiveScene().name == "SettingsSelection";
    }

    public void ChangeToPreviousScene(Character character)
    {
        //Only the first player can navigate through menu
        if(character == characters[0])
        {
            if (SceneManager.GetActiveScene().name == "SettingsMenu" || SceneManager.GetActiveScene().name == "SettingsSelection" || SceneManager.GetActiveScene().name == "PlayerSelection")
            {
                SceneManager.LoadScene(previousScene);
                ChangeScene();
            }
        }
    }

    #endregion

    #region CharactersManagement
    public void CharacterJoin()
    {
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");

        //Add the new player on the characters List
        characters[nbPlayers] = currentPlayers[nbPlayers].GetComponent<Character>();
        nbPlayers++;

        //The first player to Join need to acquire the canvas buttons on the mainMenu
        if(nbPlayers == 1 && onOnePlayerMenu)
        {
            characters[0].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
        }
    }

    public void CharacterLeft(Character character)
    {
        //Update the characters list so there is no hole on it
        for(int i = 0; i < nbPlayers; i++)
        {
            if (characters[i] == character)
            {
                //if the player 0 left, another player (if there is one) need to acquire the main button
                if(i == 0 && characters[1] != null && onOnePlayerMenu)
                {
                    characters[1].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
                }

                characters[i] = null;

                for(int j = i; j < nbPlayers; j++)
                {
                    characters[j] = characters[j + 1];
                }
            }
        }

        nbPlayers--;
    }
    #endregion
}
