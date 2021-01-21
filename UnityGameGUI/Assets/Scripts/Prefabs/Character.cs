using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Character : MonoBehaviour, UIInteract
{
    private GameObject infoPanel;
    private GameObject imagePanel;
    private Text textInfo;
    public string type;
    public General general;
    public Unit unit;

    private void Start()
    {
        infoPanel = transform.Find("GeneralInfo").gameObject;
        imagePanel = transform.Find("CurrentGeneral").gameObject;
        textInfo = infoPanel.transform.Find("Text").gameObject.GetComponent<Text>();
        textInfo.text = SetText();
        SetImage();
        infoPanel.SetActive(false);
        imagePanel.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WhenSelected();
    }

    private string SetText()
    {
        if (general != null)
        {
            return "\n" + "<b>Name: </b>" + general.name + "\n" +
            "<b>Bonus: </b>" + general.bonus + "\n" +
            "<b>GoldCost: </b>" + general.goldCost;
        }
        else if (unit != null)
        {
            return "<b>name: </b>" + unit.name + "\n" +
                "<b>maxManpower: </b>" + unit.maxManpower + "\n" +
                "<b>maxMovementSpeed: </b>" + unit.maxMovementSpeed + "\n" +
                "<b>attackEarly: </b>" + unit.attackEarly + "\n" +
                "<b>attackMid: </b>" + unit.attackMid + "\n" +
                "<b>attackLate: </b>" + unit.attackLate + "\n" +
                "<b>defenceEarly: </b>" + unit.defenceEarly + "\n" +
                "<b>defenceMid: </b>" + unit.defenceMid + "\n" +
                "<b>defenceLate: </b>" + unit.defenceLate + "\n" +
                "<b>goldCost: </b>" + unit.goldCost + "\n" +
                "<b>woodCost: </b>" + unit.woodCost + "\n" +
                "<b>foodCost: </b>" + unit.foodCost + "\n" +
                "<b>foodMaintenance: </b>" + unit.foodMaintenance + "\n" +
                "<b>goldMaintenance: </b>" + unit.goldMaintenance;
        }
        return "";
    }

    private void WhenSelected()
    {
        GameObject parent = transform.parent.gameObject;
        if(parent.name != "General" && parent.name != "AvailableUnit")
        {
            GameObject panel = parent.transform.parent.gameObject.
            transform.parent.gameObject.
            transform.parent.gameObject;
            if (type == "General")
            {
                parent.GetComponent<SelectCharacter>().general = general;
            }
            else
            {
                parent.GetComponent<SelectCharacter>().unit = unit;
            }
            panel.SetActive(false);
        }

        if(parent.name == "AvailableUnit")
        {
            parent.GetComponent<CreateUnit>().selectedUnit = unit;
        }
    }

    public void SetImage()
    {
        if (type == "General")
        {
            imagePanel.GetComponent<Image>().sprite = general.image;
        }
        else
        {
            imagePanel.GetComponent<Image>().sprite = unit.image;
        }
    }
}
