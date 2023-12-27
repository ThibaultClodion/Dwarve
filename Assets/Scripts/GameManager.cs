using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Characters Data
    private Character[] characters = new Character[4];
    private int nbCharacters = 0;

    //Player Data
    private bool[] isAlive = new bool[4];
    private int[] nbVictory = new int[4];

    //One player canvas Menu
    private bool onOneCharacterMenu;
    private GameObject oneCharacterButton;

    //Multi Canvas Menu
    private bool onMultiCharacterMenu;
    private GameObject[] multiCharacterCanvas = new GameObject[4];
    private GameObject[][] disableObjectsCharacters = new GameObject[4][];

    //In Game
    private bool inGameScene;
    private int NbRoundToWin = 2;

    //Scene Memory
    private string actualScene;
    private string previousScene;

    private void Start()
    {
        //Start is consider as Changing Scene
        actualScene = SceneManager.GetActiveScene().name;
        previousScene = SceneManager.GetActiveScene().name;

        //Initialize the wins
        for(int i = 0; i < 4; i++)
        {
            nbVictory[i] = 0;
        }

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

        //If the scene is a one character canvas
        if (onOneCharacterMenu)
        {
            //Find the main bouton even if there is no character because he may comes latter
            oneCharacterButton = GameObject.FindGameObjectWithTag("MainButton");

            if(FindTheMainCharacter() != - 1)
            {
                characters[FindTheMainCharacter()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(oneCharacterButton);
            }
        }

        //If the scene is a multi character canvas
        else if(onMultiCharacterMenu) 
        {
            //Get the character Canvas even if they are Disable on the scene
            int i = 0;
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects) 
            {
                if(obj.tag == "PlayerCanvas")
                {
                    //Be careful that the canvas names are 1,2,3,4
                    multiCharacterCanvas[int.Parse(obj.name) - 1] = obj;
                    i++;
                }
            }

            //Activate characters Canvas
            for(int j = 0; j < 4; j++)
            {
                ActivateCharacterICanvas(j);
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
                    isAlive[i] = true;
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

        //To not be able to return victory menu
        if(previousScene == "VictoryScene")
        {
            previousScene = "MainMenu";
        }

        onOneCharacterMenu = SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "SettingsMenu" || SceneManager.GetActiveScene().name == "SettingsSelection";
        onMultiCharacterMenu = SceneManager.GetActiveScene().name == "PlayerSelection" || SceneManager.GetActiveScene().name == "WeaponModding" || SceneManager.GetActiveScene().name == "VictoryScene";
        inGameScene = SceneManager.GetActiveScene().name.Contains("Map");

        //The character are no longer ready because the scene change
        for (int i = 0; i < 4; i++) 
        {
            if (characters[i] != null)
            {
                CharacterIsNotReady(i);
            }

            //Consider that no one is Alive when changing scene
            isAlive[i] = false;

            //Reset the number of win if on a non inGame Scene
            if(onOneCharacterMenu)
            {
                nbVictory[i] = 0;
            }
        }
    }

    IEnumerator VictorySceneUpdate(int indewWinner)
    {
        //Wait for loading the scene
        yield return new WaitForSeconds(0.1f);

        //Transfer information to the VictoryScene (provisory)
        GameObject.Find("Victory Text").GetComponent<TextMeshProUGUI>().text = "Player n°" + indewWinner + " win";

    }

    private void UpdateLayoutSize()
    {
        GameObject layoutGroup = GameObject.Find("GroupLayout");

        if(layoutGroup != null) 
        {
            layoutGroup.GetComponent<AdaptLayoutSize>().UpdateSize();
        }
    }


    public int FindTheMainCharacter()
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

    public void ActivateCharacterICanvas(int i)
    {
        if (characters[i] != null && onMultiCharacterMenu && multiCharacterCanvas[i] != null)
        {
            //Activate the canvas
            multiCharacterCanvas[i].transform.parent.gameObject.SetActive(true);

            //Make the character access to the buttons
            characters[i].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(multiCharacterCanvas[i]);

            //Update the layout to the number of character
            UpdateLayoutSize();

            //The Weapon Modding menu have a different buttons system
            if(SceneManager.GetActiveScene().name == "WeaponModding")
            {
                //Get a weapon button
                GameObject button = characters[i].InitWeaponModding(multiCharacterCanvas[i]);

                //Make the character access to the weapon button
                characters[i].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(button);
            }
        }
    }

    public void DesactivateCharacterICanvas(int i)
    {
        if (characters[i] != null && onMultiCharacterMenu && multiCharacterCanvas[i] != null)
        {
            //Desactivate the canvas
            multiCharacterCanvas[i].transform.parent.gameObject.SetActive(false);
        }
    }

    public void ChangeToPreviousScene(Character character)
    {
        //Only the first character can navigate through menu
        if(character == characters[FindTheMainCharacter()])
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
                characters[i] = currentPlayers[nbCharacters].GetComponent<Character>();

                //If the scene is multiCanvas, the character acquire his canvas
                if (onMultiCharacterMenu)
                {
                    ActivateCharacterICanvas(i);
                }

                break;
            }
        }
        nbCharacters++;

        //The first character to Join need to acquire the canvas buttons on the mainMenu
        if(onOneCharacterMenu)
        {
            characters[FindTheMainCharacter()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(oneCharacterButton);
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
                //If the scene is multiCanvas, the character desacquire it
                if (onMultiCharacterMenu)
                {
                    DesactivateCharacterICanvas(i);
                }

                characters[i] = null;

                //Another player need to acquire the button if the scene is a single player canvas
                if (i == 0 && FindTheMainCharacter() != -1 && onOneCharacterMenu)
                {
                    characters[FindTheMainCharacter()].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(oneCharacterButton);
                }
            }
        }

        nbCharacters--;
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

    public void CharacterIsReady(int i, GameObject[] disableObjects, string nextScene)
    {
        if (characters[i] != null)
        {
            characters[i].isReadyForNextScene = true;

            //Save the disablesObjects in case the character want to reactivate them
            disableObjectsCharacters[i] = disableObjects;

            for (int j = 0; j < disableObjects.Length; j++)
            {
                disableObjects[j].SetActive(false);
            }

            if(getNbReady() == nbCharacters)
            {
                ChangeScene();
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void WeaponIsReady(Character character)
    {
        //Call when the character is on the weapon modding scene

        int i = FindCharacter(character);

        if (i != -1)
        {
            characters[i].isReadyForNextScene = true;

            multiCharacterCanvas[i].SetActive(false);

            if (getNbReady() == nbCharacters)
            {
                ChangeScene();
                SceneManager.LoadScene("Map 0");
            }
        }
    }

    public void WeaponIsNotReady(Character character)
    {
        int i = FindCharacter(character);

        if(i != -1)
        {
            characters[i].isReadyForNextScene = false;

            multiCharacterCanvas[i].SetActive(true);
        }
    }

    public void CharacterIsNotReady(Character character)
    {
        //Back when the character wasn't ready

        int i = FindCharacter(character);

        if(i != -1)
        {
            characters[i].isReadyForNextScene = false;
            
            for(int j = 0; j < disableObjectsCharacters[i].Length; j++)
            {
                disableObjectsCharacters[i][j].SetActive(true);
            }
        }

    }

    private void CharacterIsNotReady(int i)
    {
       characters[i].isReadyForNextScene = false;
    }
    #endregion

    #region PlayersManagement

    private int nbAlive()
    {
        int i = 0;
        for(int j = 0; j < 4; j++)
        {
            if (characters[j] != null && isAlive[j])
            {
                i++;
            }
        }
        return i;
    }

    public void PlayerDie(Character character)
    {
        int i = FindCharacter(character);
        isAlive[i] = false;

        if(nbAlive() == 1)
        {
            Victory();
        }

    }

    private void Victory()
    {
        //Find the Winner
        int indexWinner = 0;
        for(int i = 0; i < 4; i++)
        {
            if (isAlive[i])
            {
                indexWinner = i;
            }

        }
        //Disable the player
        characters[indexWinner].DisablePlayer();
        //Increase his win count
        nbVictory[indexWinner]++;

        //If the player win the game than end it
        if (nbVictory[indexWinner] == NbRoundToWin)
        {
            SceneManager.LoadScene("VictoryScene");
            StartCoroutine(VictorySceneUpdate(indexWinner));
            ChangeScene();
        }
        //Else go to the weapon editing menu
        else
        {
            SceneManager.LoadScene("WeaponModding");
            ChangeScene();
        }
    }

    #endregion
}
