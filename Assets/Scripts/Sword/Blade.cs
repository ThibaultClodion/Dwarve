using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
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
        if (other.gameObject.tag == "PlayerPrefab" && other.gameObject != playerGO)
        {
            other.gameObject.GetComponent<PlayerController>().Hit();
        }
    }
}
