using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInformation : MonoBehaviour
{
    public UnitArmy unit;
    public string army;
    public Image image;
    public Text infoText;
    public Text costsText;
    public Button reinforceButton;
    private RegionHandler region;

    public GameObject toastObj;

    private void Start()
    {
        costsText.gameObject.SetActive(false);
        reinforceButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        getCurrentRegion();

        if (unit != null)
        {
            if(unit.availableManPower < unit.unit.maxManpower 
                && region.isMoving == false && unit.reinforcing == false && InfoManager.Instance.updating == false)
            {
                costsText.gameObject.SetActive(true);
                reinforceButton.gameObject.SetActive(true);
            }
            else
            {
                costsText.gameObject.SetActive(false);
                reinforceButton.gameObject.SetActive(false);
            }
            SetPanelInfo();
        }
    }

    private void OnDisable()
    {
        costsText.gameObject.SetActive(false);
        reinforceButton.gameObject.SetActive(false);
        unit = null;
        army = "";
        image = null;
        costsText.text = "";
        infoText.text = "";
    }

    public void ReinforceUnit()
    {
        Toast toastManager = toastObj.GetComponent<Toast>();

        if (Utils.GetMyFaction().gold >= unit.reinforcementCosts)
        {
            Utils.GetMyFaction().gold -= unit.reinforcementCosts;

            ServerResponse response = new ServerResponse();
            response.EventType = "ReinforceUnit";
            response.data.Add(unit.id);
            response.data.Add(army);
            GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

            region.isMoving = true;
            unit.reinforcing = true;
            costsText.text = "<b>REINFORCING</b>";
            reinforceButton.gameObject.SetActive(false);
        }
        else
        {
            toastManager.toasts.Enqueue("Don't have enough resources");
        }
    }

    private void SetPanelInfo()
    {
        if(unit.reinforcing == false)
        {
            costsText.text = "<b>Costs: </b>" + unit.reinforcementCosts;
        }
        else
        {
            costsText.text = "<b>REINFORCING</b>";
        }

        infoText.text = "<b>Name: </b>" + unit.unit.name + "\n" +
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

        image.sprite = unit.unit.image;
    }

    private void getCurrentRegion()
    {
        MapManager map = new MapManager();
        List<GameObject> regionsObj = map.GetRegions();
        foreach (GameObject reg in regionsObj)
        {
            if (reg.GetComponent<RegionHandler>().region.name == InfoManager.Instance.selectedRegion)
            {
                region = reg.GetComponent<RegionHandler>();
            }
        }
    }
}
