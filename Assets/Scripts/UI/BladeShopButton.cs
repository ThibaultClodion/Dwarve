using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class BladeShopButton : MonoBehaviour
{

    public void OpenShop()
    {
        //Find the Character and his sword to open the Shop

        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Character character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.name) - 1);
        Sword sword = character.GetComponent<Sword>();

        sword.OpenShop();
    }
}
