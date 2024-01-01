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

    [Header("Object Datas")]
    [SerializeField] private Sprite objectSprite;
    [SerializeField] private int price;
    [SerializeField] private string Objectname;
    #endregion

    private void Awake()
    {
        //Get the number of space
        int nbSpace = 4 - price.ToString().Length;
        string space = "";
        for(int i = 0; i < nbSpace; i++) 
        {
            space += "  ";
        }

        //Change the button Text
        buttonText.text = price.ToString() +  space + "| " + Objectname;
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        //Change the scrollbarValue
        float nbButtons = transform.parent.childCount;
        scrollbar.value = 1 - (int.Parse(this.transform.name)) / (nbButtons - 1);

        //Change the description
        image.sprite = objectSprite;
    }
}
