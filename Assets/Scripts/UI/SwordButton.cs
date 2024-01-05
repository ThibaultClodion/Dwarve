using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class SwordButton : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    public bool isBlade;
    public bool canBeSelect;
    [NonSerialized] public int bladeIndex = 0;

    private Sword sword;

    private void Start()
    {
        //Find the Character and his sword
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Character character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.name) - 1);
        sword = character.GetSword();
    }
    public void OnSelect(BaseEventData eventData)
    {
        if(canBeSelect)
        {
            if (sword == null)
            {
                //Find the Character and his sword
                GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

                Character character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.name) - 1);
                sword = character.GetSword();
            }
            if (sword.isSwitchingBlade && isBlade)
            {
                sword.SelectSwitch();
            }
        }
        else
        {
            canBeSelect = true;
        }
    }

    public void OpenShop()
    {
        if(sword != null && !sword.isSwitchingBlade) 
        {
            sword.OpenShop(bladeIndex, isBlade);
        }
        else if(sword != null && sword.isSwitchingBlade)
        {
            sword.ConfirmSwitchBlade();
        }
    }
}
