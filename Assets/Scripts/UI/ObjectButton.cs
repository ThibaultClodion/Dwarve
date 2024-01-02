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
    [SerializeField] private bool isABlade;
    #endregion

    private void Awake()
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

    private void Start()
    {
        //Find the Character
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        character = gameManager.GetICharacter(int.Parse(this.transform.parent.parent.parent.parent.parent.name) - 1);

        //Change the button color if the character has not enough money
        if(character.GetMoney() < blade.price) 
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        //Change the scrollbarValue
        float nbButtons = transform.parent.childCount;
        scrollbar.value = 1 - (int.Parse(this.transform.name)) / (nbButtons - 1);

        //Change the description
        image.sprite = blade.sprite;
    }

    //Do this when the Button is Click
    public void OnClick(BaseEventData eventData) 
    {
        if (character.GetMoney() > blade.price)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }
}
