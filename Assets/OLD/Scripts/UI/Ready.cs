using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Ready : MonoBehaviour
{
    public static int nbReady;
    public bool isReady = false;

    private bool isInSelection = false;
    [SerializeField] private GameObject[] selectionDisapear;

    private bool isModdingWeapon = false;
    [SerializeField] private GameObject[] weaponDisapear;

    private int nbPlayers;

    public void UpdateReady()
    {
        if(SceneManager.GetActiveScene().name == "PlayerSelection")
        {
            isInSelection = true;
            isModdingWeapon = false;

            nbPlayers = GameObject.Find("PlayerManager").GetComponent<PlayerInputManager>().playerCount;
        }
        else if(SceneManager.GetActiveScene().name == "WeaponSelection")
        {
            isInSelection = false;
            isModdingWeapon = true;

            nbPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        }
    }

    public void ResetNbReady()
    {
        nbReady = 0;
        isReady = false;
    }

    public void IsReady()
    {
        UpdateReady();

        if(!isReady)
        {
            nbReady++;
            isReady = true;

            if (isInSelection)
            {
                for (int i = 0; i < selectionDisapear.Length; i++)
                {
                    selectionDisapear[i].SetActive(false);
                }
            }
            else if(isModdingWeapon)
            {
                for (int i = 0; i < weaponDisapear.Length; i++)
                {
                    weaponDisapear[i].SetActive(false);
                }
            }
        }

        if (nbReady == nbPlayers && isReady && isInSelection)
        {
            SceneManager.LoadScene("TestScene");
        }
        else if (nbReady == nbPlayers && isReady && isModdingWeapon)
        {
            Debug.Log("Test");
            SceneManager.LoadScene("TestScene");
        }
    }

    public void NotReady()
    {
        UpdateReady();

        if (isReady) 
        {
            nbReady--;
            isReady = false;

            if (isInSelection)
            {
                for (int i = 0; i < selectionDisapear.Length; i++)
                {
                    selectionDisapear[i].SetActive(true);
                }
            }
            else if (isModdingWeapon)
            {
                for (int i = 0; i < weaponDisapear.Length; i++)
                {
                    weaponDisapear[i].SetActive(true);
                }
            }
        }
    }

    void OnCancel()
    {
        if (isInSelection)
        {
            NotReady();
        }

        if(isModdingWeapon && isReady)
        {
            NotReady();
        }
        else if(isModdingWeapon &&  !isReady)
        {
            IsReady();
        }
    }
}
