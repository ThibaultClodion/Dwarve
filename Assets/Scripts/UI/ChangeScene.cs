using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //Change scene (to change index go to build settings ! )
    public void MoveToScene(string name)
    {
       SceneManager.LoadScene(name);
    }
}
