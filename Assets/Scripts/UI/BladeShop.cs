using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class BladeShop : MonoBehaviour
{
    [SerializeField] private GameObject shop;
    private GameManager gameManager;
    private Character character;
    private Sword sword;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void OpenShop()
    {
        UpdateCharacterDatas();

        shop = Instantiate(shop);
        shop.transform.SetParent(this.transform.parent.parent.parent, false);

        //Destroy the old weapon
        sword.DestroyOldWeapon();

        //Set the selected button of the character
        character.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(shop);
    }

    private void UpdateCharacterDatas()
    {
        character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.name) - 1);
        sword = character.GetComponent<Sword>();
    }
}
