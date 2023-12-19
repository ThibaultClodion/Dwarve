using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(string name)
    {
        //Update here the GameManager because button can't acces his script
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ChangeScene();

        //Load the new Scene
        SceneManager.LoadScene(name);
    }
}
