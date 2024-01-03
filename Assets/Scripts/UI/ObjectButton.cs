using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    #region Datas
    [Header("Canvas Datas")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI buttonText;

    //Character Datas
    private Character character;

    [Header("Object Datas")]
    [SerializeField] private BladeData blade;
    [SerializeField] private HiltData hilt;
    [SerializeField] private bool isABladeShop;
    #endregion

    private void Awake()
    {
        if(isABladeShop) 
        {
            //Get the number of space
            int nbSpace = 4 - blade.price.ToString().Length;
            string space = "";
            for (int i = 0; i < nbSpace; i++)
            {
                space += "  ";
            }

            //Change the button Text
            buttonText.text = blade.price.ToString() + space + "| " + blade.nameDisplay;
        }
        else
        {
            //Get the number of space
            int nbSpace = 4 - hilt.price.ToString().Length;
            string space = "";
            for (int i = 0; i < nbSpace; i++)
            {
                space += "  ";
            }

            //Change the button Text
            buttonText.text = hilt.price.ToString() + space + "| " + hilt.nameDisplay;
        }
    }

    private void Start()
    {
        //Find the Character
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.parent.parent.name) - 1);

        if(isABladeShop)
        {
            //Change the button color if the character has not enough money
            if (character.GetMoney() < blade.price)
            {
                GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }
        }
        else
        {
            //Change the button color if the character has not enough money
            if (character.GetMoney() < hilt.price)
            {
                GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        //Change the scrollbarValue
        float nbButtons = transform.parent.childCount;
        scrollbar.value = 1 - (int.Parse(this.transform.name)) / (nbButtons - 1);

        //Change the description
        if (isABladeShop)
        {
            image.sprite = blade.sprite;
        }
        else
        {
            image.sprite = hilt.sprite;
        }
    }

    public void BuyObject() 
    {
        if(isABladeShop)
        {
            if (character.GetMoney() >= blade.price)
            {
                //Change the blade and update the money
                character.GetSword().ChangeBlade(blade);
                character.MoneyExchange(-blade.price);

                //Close the shop to visualize the new weapon
                character.GetSword().CloseShop();
            }
        }
        else
        {
            if (character.GetMoney() >= hilt.price)
            {
                //Change the hilt and update the money
                character.GetSword().ChangeHilt(hilt);
                character.MoneyExchange(-hilt.price);

                //Close the shop to visualize the new weapon
                character.GetSword().CloseShop();
            }
        }
    }
}
