using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectPrefab : MonoBehaviour, UIInteract
{
    private GameObject plusSignPanel;
    private GameObject infoPanel;
    private GameObject imagePanel;
    private GameObject chooseChar;
    private Text text;
    private GameObject selectList;
    private Image sprite;
    private Color defaultColor;

    public General general;
    public Unit unit;

    public string type;
    private bool selected;

    void Awake()
    {
        chooseChar = transform.parent.gameObject.transform.parent.gameObject.GetComponent<CreateArmy>().createChar;
        GameObject obj = chooseChar.transform.Find("List Avaiable").gameObject;
        obj = obj.transform.Find("Viewport").gameObject;
        selectList = obj.transform.Find("Content").gameObject;

        plusSignPanel = transform.Find("AddGeneral").gameObject;
        infoPanel = transform.Find("GeneralInfo").gameObject;
        imagePanel = transform.Find("CurrentGeneral").gameObject;

        text = infoPanel.transform.Find("Text").gameObject.GetComponent<Text>();
        sprite = GetComponent<Image>();

        defaultColor = sprite.color;
        plusSignPanel.SetActive(true);
        infoPanel.SetActive(false);
        imagePanel.SetActive(false);
        general = null;
        unit = null;
        selected = false;
    }

    private void Update()
    {
        if (general == null && unit == null)
        {
            if(selected == true)
            {
                GetSelected();
            }

            text.text = "";
            imagePanel.GetComponent<Image>().sprite = null;

            plusSignPanel.SetActive(true);
            imagePanel.SetActive(false);
        }
        if (general != null || unit != null)
        {
            selected = false;

            SetText();
            SetImage();

            plusSignPanel.SetActive(false);
            imagePanel.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        general = null;
        unit = null;
        selected = true;
        selectList.GetComponent<SelectCharacter>().type = type;
        chooseChar.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (plusSignPanel.activeSelf)
        {
            sprite.color = new Color(0.5849056f, 0.2289961f, 0.2289961f, 1);
        }
        if (imagePanel.activeSelf)
        {
            infoPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (plusSignPanel.activeSelf)
        {
            sprite.color = defaultColor;
        }
        if (imagePanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
    }

    private void GetSelected()
    {

        if(type == "General")
        {
            general = selectList.GetComponent<SelectCharacter>().general;
        }
        else
        {
            unit = selectList.GetComponent<SelectCharacter>().unit;
        }
    }

    private void SetText()
    {
        if(type == "General")
        {
            text.text = "\n" + "<b>Name: </b>" + general.name + "\n" +
                "<b>Bonus: </b>" + general.bonus + "\n" +
                "<b>GoldCost: </b>" + general.goldCost;
        }
        else
        {
            text.text = "<b>name: </b>" + unit.name + "\n" +
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
