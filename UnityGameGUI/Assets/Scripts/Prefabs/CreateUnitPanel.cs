using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreateUnitPanel : MonoBehaviour, UIInteract
{
    public GameObject createUnitPanel;

    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        createUnitPanel.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void closeAddUnit()
    {
        createUnitPanel.SetActive(false);
    }
}
