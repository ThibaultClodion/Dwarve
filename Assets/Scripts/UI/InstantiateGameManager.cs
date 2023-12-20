using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    private void Awake()
    {
        //Auto-Destruction if there is already a GameManager is the scene
        if (GameObject.FindGameObjectsWithTag("GameManager").Length == 0)
        {
            Instantiate(gameManager);
        }
    }
}
