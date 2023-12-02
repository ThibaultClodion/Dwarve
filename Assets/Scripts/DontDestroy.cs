using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    //Don't destroy this item
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
