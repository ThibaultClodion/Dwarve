using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class ShopButton : MonoBehaviour
{
    public bool isBladeShop;
    [NonSerialized] public int bladeIndex = 0;

    public void OpenShop()
    {
        //Find the Character and his sword to open the Shop

        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Character character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.name) - 1);
        Sword sword = character.GetSword();

        sword.OpenShop(bladeIndex, isBladeShop);
    }
}
