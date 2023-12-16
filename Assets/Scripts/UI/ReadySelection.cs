using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReadySelection : MonoBehaviour
{
    public static int nbReady;
    public bool isReady = false;
    [SerializeField] private GameObject[] readyDisapear;

    private PlayerInputManager inputManager;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            inputManager = GameObject.Find("PlayerManager").GetComponent<PlayerInputManager>();
        }
    }

    public void IsReady()
    {
        if(!isReady)
        {
            nbReady++;
            isReady = true;

            for(int i = 0; i < readyDisapear.Length; i++) 
            {
                readyDisapear[i].SetActive(false);
            }
        }

        if(nbReady == inputManager.playerCount && isReady)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void NotReady()
    {
        if(isReady) 
        {
            nbReady--;
            isReady = false;

            for (int i = 0; i < readyDisapear.Length; i++)
            {
                readyDisapear[i].SetActive(true);
            }
        }
    }

    void OnCancel()
    {
        //Otherwise the script is call during the game
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            NotReady();
        }
    }
}
