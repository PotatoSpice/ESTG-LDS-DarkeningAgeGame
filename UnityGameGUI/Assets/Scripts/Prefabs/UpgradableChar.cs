using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradableChar : MonoBehaviour, UIInteract
{
    private GameObject infoPanel;
    private GameObject imagePanel;
    private GameObject checkUnit;
    private Slider lifeBar;
    private Text textLife;
    private Text textInfo;

    public UnitArmy unit;
    public string army;

    void Start()
    {
        imagePanel = transform.Find("UnitImage").gameObject;
        infoPanel = transform.Find("UnitInfo").gameObject;
        textInfo = infoPanel.transform.Find("Text").gameObject.GetComponent<Text>();
        lifeBar = imagePanel.transform.Find("LifeBar").gameObject.GetComponent<Slider>();
        textLife = imagePanel.transform.Find("LifeBar").gameObject.transform.Find("LifeValue").gameObject.GetComponent<Text>();
        checkUnit = transform.parent.gameObject.transform.parent.gameObject
            .transform.parent.gameObject.transform.parent.gameObject
            .transform.parent.gameObject.transform.parent.gameObject.transform.Find("CheckUnit").gameObject;

        SetText();
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
        checkUnit.GetComponent<UnitInformation>().unit = unit;
        checkUnit.GetComponent<UnitInformation>().army = army;
        checkUnit.SetActive(true);
    }

    public void SetText()
    {
        textInfo.text = "<b>Name: </b>" + unit.unit.name + "\n" +
                "<b>Max Manpower: </b>" + unit.unit.maxManpower + "\n" +
                "<b>Max MovementSpeed: </b>" + unit.unit.maxMovementSpeed + "\n" +
                "<b>Attack Early: </b>" + unit.unit.attackEarly + "\n" +
                "<b>Attack Mid: </b>" + unit.unit.attackMid + "\n" +
                "<b>Attack Late: </b>" + unit.unit.attackLate + "\n" +
                "<b>Defence Early: </b>" + unit.unit.defenceEarly + "\n" +
                "<b>Defence Mid: </b>" + unit.unit.defenceMid + "\n" +
                "<b>Defence Late: </b>" + unit.unit.defenceLate + "\n" +
                "<b>Gold Cost: </b>" + unit.unit.goldCost + "\n" +
                "<b>Wood Cost: </b>" + unit.unit.woodCost + "\n" +
                "<b>Food Cost: </b>" + unit.unit.foodCost + "\n" +
                "<b>Food Maintenance: </b>" + unit.unit.foodMaintenance + "\n" +
                "<b>Gold Maintenance: </b>" + unit.unit.goldMaintenance + "\n" +
                "<b>Available ManPower: </b>" + unit.availableManPower + "\n" +
                "<b>Reinforcement Costs: </b>" + unit.reinforcementCosts;

        textLife.text = unit.availableManPower + "/" + unit.unit.maxManpower;
        lifeBar.value = unit.availableManPower / unit.unit.maxManpower;
    }

    public void SetImage()
    {
        imagePanel.GetComponent<Image>().sprite = unit.unit.image;
    }
}