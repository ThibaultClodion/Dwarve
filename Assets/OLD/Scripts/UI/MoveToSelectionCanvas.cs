using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToSelectionCanvas : MonoBehaviour
{
    void Awake()
    {
        GameObject mainCanvas = GameObject.Find("SelectionCanvas");

        //If we are in the seleciton menu than move the canvas to group all the canvas
        if (mainCanvas != null )
        {
            this.transform.SetParent(mainCanvas.transform, false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
