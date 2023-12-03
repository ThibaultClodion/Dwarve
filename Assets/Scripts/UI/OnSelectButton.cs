using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class OnSelectButton : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    [SerializeField] private SkinSelection skinSelection;
    [SerializeField] private bool IsIncrement;

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (IsIncrement) 
        {
            skinSelection.IncrementIndex();
        }
        else
        {
            skinSelection.DecrementIndex();
        }
    }
}
