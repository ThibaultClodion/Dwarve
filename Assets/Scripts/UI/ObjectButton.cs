using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    //In Canvas Datas
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    //Object Datas
    [SerializeField] private Sprite objectSprite;
    [SerializeField] private string objectDescription;

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        //Change the scrollbarValue
        float nbButtons = transform.parent.childCount;
        scrollbar.value = 1 - (int.Parse(this.transform.name)) / (nbButtons - 1);

        //Change the description
        image.sprite = objectSprite;
        text.text = objectDescription;

    }
}
