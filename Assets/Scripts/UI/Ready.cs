using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ready : MonoBehaviour
{
    public static int nbReady;

    public void IsReady()
    {
        nbReady++;
        Debug.Log(nbReady);
    }
}
