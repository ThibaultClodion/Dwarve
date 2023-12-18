using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        //This is use to be able to exit even if the project is not build
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
