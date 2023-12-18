using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private GameObject playerGO;


    // Start is called before the first frame update
    void Start()
    {
        //Search the player GO do don't hit himself
        playerGO = this.transform.parent.parent.parent.gameObject;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Trigger with another Player
        if (other.gameObject.tag == "Player" && other.gameObject != playerGO)
        {
            other.gameObject.GetComponent<CharacterOld>().Hit();
        }
    }
}