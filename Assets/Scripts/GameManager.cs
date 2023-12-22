using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Characters Data
    private Character[] characters = new Character[4];
    private int nbPlayers = 0;

    //One player canvas Menu
    private bool onOnePlayerMenu;
    private GameObject onePlayerButton;

    //Multi Canvas Menu
    private bool onMultiPlayerMenu;
    private GameObject[] multiPlayerCanvas = new GameObject[4];
    private GameObject[][] disableObjectsPlayers = new GameObject[4][];

    //In Game
    private bool inGameScene;

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
        UpdateSceneDatas();

        //If the scene is a one player canvas
        if (onOnePlayerMenu)
        {
            //Find the main bouton even if there is no player because he may comes latter
            onePlayerButton = GameObject.FindGameObjectWithTag("MainButton");

            if(FindTheMainPlayer() != - 1)
            {
                characters[FindTheMainPlayer()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
            }
        }

        //If the scene is a multi player canvas
        else if(onMultiPlayerMenu) 
        {
            //Get the Player Canvas even if they are Disable on the scene
            int i = 0;
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects) 
            {
                if(obj.tag == "PlayerCanvas")
                {
                    //Be careful that the canvas names are 1,2,3,4
                    multiPlayerCanvas[int.Parse(obj.name) - 1] = obj;
                    i++;
                }
            }

            //Activate the player Canvas
            for(int j = 0; j < 4; j++)
            {
                ActivatePlayerICanvas(j);
            }
        }

        //If the scene is a Game scne
        else if(inGameScene)
        {
            GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

            for (int i = 0; i < 4; i++)
            {
                if (characters[i] != null)
                {
                    characters[i].InitPlayer(spawns[i].transform.position);
                }
            }

            //Update the cinemachine target group component
            GameObject targetGroup = GameObject.Find("Target Group");
            if(targetGroup != null)
            {
                targetGroup.GetComponent<TargetGroupAutomatic>().UpdateTarget();
            }
        }
    }
    public void UpdateSceneDatas()
    {
        previousScene = actualScene;
        actualScene = SceneManager.GetActiveScene().name;
        onOnePlayerMenu = SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "SettingsMenu" || SceneManager.GetActiveScene().name == "SettingsSelection";
        onMultiPlayerMenu = SceneManager.GetActiveScene().name == "PlayerSelection" || SceneManager.GetActiveScene().name == "WeaponModding";
        inGameScene = SceneManager.GetActiveScene().name.Contains("Map");

        //The player are no longer ready because the scene change
        for (int i = 0; i < 4; i++) 
        {
            if (characters[i] != null)
            {
                PlayerIsNotReady(i);
            }
        }
    }

    private void UpdateLayoutSize()
    {
        GameObject layoutGroup = GameObject.Find("GroupLayout");

        if(layoutGroup != null) 
        {
            layoutGroup.GetComponent<AdaptLayoutSize>().UpdateSize();
        }
    }


    public int FindTheMainPlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            if (characters[i] != null)
            {
                return i;
            }
        }
        return -1;
    }

    public void ActivatePlayerICanvas(int i)
    {
        if (characters[i] != null && onMultiPlayerMenu && multiPlayerCanvas[i] != null)
        {
            //Activate the canvas
            multiPlayerCanvas[i].transform.parent.gameObject.SetActive(true);
            //Make the player access to the buttons
            characters[i].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(multiPlayerCanvas[i]);

            //Update the layout to the number of player
            UpdateLayoutSize();
        }
    }

    public void DesactivatePlayerICanvas(int i)
    {
        if (characters[i] != null && onMultiPlayerMenu && multiPlayerCanvas[i] != null)
        {
            //Desactivate the canvas
            multiPlayerCanvas[i].transform.parent.gameObject.SetActive(false);
        }
    }

    public void ChangeToPreviousScene(Character character)
    {
        //Only the first player can navigate through menu
        if(character == characters[FindTheMainPlayer()])
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

        //Find a place on the list of characters
        for(int i = 0; i < 4; i++) 
        {
            if (characters[i] == null)
            {
                //Add the new player on the characters List
                characters[i] = currentPlayers[nbPlayers].GetComponent<Character>();

                //If the scene is multiCanvas, the player acquire his canvas
                if (onMultiPlayerMenu)
                {
                    ActivatePlayerICanvas(i);
                }

                break;
            }
        }
        nbPlayers++;

        //The first player to Join need to acquire the canvas buttons on the mainMenu
        if(onOnePlayerMenu)
        {
            characters[FindTheMainPlayer()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
        }
    }

    public void CharacterLeft(Character character)
    {
        int i = FindCharacter(character);

        //Update the characters list so there is no hole on it
        if(i == -1)
        {
            if (characters[i] == character)
            {
                //If the scene is multiCanvas, the player desacquire it
                if (onMultiPlayerMenu)
                {
                    DesactivatePlayerICanvas(i);
                }

                characters[i] = null;

                //Another player need to acquire the button if the scene is a single player canvas
                if (i == 0 && FindTheMainPlayer() != -1 && onOnePlayerMenu)
                {
                    characters[FindTheMainPlayer()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(onePlayerButton);
                }
            }
        }

        nbPlayers--;
    }

    private int getNbReady()
    {
        int i = 0;
        for(int j = 0; j < 4; j++) 
        {
            if (characters[j] != null && characters[j].isReadyForNextScene)
            {
                i++;
            }
        }

        return i;
    }

    private int FindCharacter(Character character)
    {
        //Find the character in the array 
        for(int i = 0; i < 4; i++)
        {
            if (characters[i] == character)
            {
                return i;
            }
        }
        return -1;
    }

    public void PlayerIsReady(int i, GameObject[] disableObjects, string nextScene)
    {
        if (characters[i] != null)
        {
            characters[i].isReadyForNextScene = true;

            //Save the disablesObjects in case the player want to reactivate them
            disableObjectsPlayers[i] = disableObjects;

            for (int j = 0; j < disableObjects.Length; j++)
            {
                disableObjects[j].SetActive(false);
            }

            if(getNbReady() == nbPlayers)
            {
                ChangeScene();
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void PlayerIsNotReady(Character character)
    {
        //Back when the player wasn't ready

        int i = FindCharacter(character);

        if(i != -1)
        {
            characters[i].isReadyForNextScene = false;
            
            for(int j = 0; j < disableObjectsPlayers[i].Length; j++)
            {
                disableObjectsPlayers[i][j].SetActive(true);
            }
        }

    }

    private void PlayerIsNotReady(int i)
    {
       characters[i].isReadyForNextScene = false;
    }
    #endregion
}
