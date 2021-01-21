using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateUnit : MonoBehaviour
{
    public Text selected;
    public GameObject prefabUnit;
    public GameObject checkArmy;
    public GameObject parentT3Panel;

    private List<Unit> optionsUnitList;
    private List<GameObject> prefabsList;
    public Unit selectedUnit;

    public GameObject toastObj;

    void Awake()
    {
        optionsUnitList = new List<Unit>();
        prefabsList = new List<GameObject>();
    }
    private void Update()
    {
        if (selectedUnit != null)
        {
            selected.text = "Selected: " + selectedUnit.name;
        }
    }

    private void OnEnable()
    {
        selectedUnit = null;
        selected.text = "Selected: ";
        GetCharacters();
    }

    private void OnDisable()
    {
        selectedUnit = null;
        selected.text = "Selected: ";
        optionsUnitList.Clear();
        foreach (GameObject obj in prefabsList)
        {
            Destroy(obj);
        }
    }

    private void GetCharacters()
    {
        optionsUnitList.Clear();
        prefabsList.Clear();

        foreach (Unit unit in InfoManager.Instance.armyCollections.getUnits())
        {
            GameObject newObj;
            newObj = (Instantiate(prefabUnit, transform));
            newObj.GetComponent<Character>().type = "Unit";
            newObj.GetComponent<Character>().unit = unit;
            prefabsList.Add(newObj);
            optionsUnitList.Add(unit);
        }
    }

    public void ConfirmSelection()
    {
        if (selectedUnit != null)
        {
            Toast toastManager = toastObj.GetComponent<Toast>();

            //retirar aos recursos
            Faction myFaction = Utils.GetMyFaction();
            if(myFaction.food >= selectedUnit.foodCost && myFaction.gold >= selectedUnit.goldCost && myFaction.wood >= selectedUnit.woodCost)
            {
                UnitArmy unitArmy = new UnitArmy();
                unitArmy.unit = selectedUnit;
                unitArmy.availableManPower = selectedUnit.maxManpower;
                unitArmy.reinforcementCosts = 0;

                checkArmy.GetComponent<CheckArmy>().army.units.Add(unitArmy);
                checkArmy.GetComponent<CheckArmy>().army.unitCount += 1;
                checkArmy.GetComponent<CheckArmy>().army.totalUnitCount += 1;

                myFaction.food -= selectedUnit.foodCost;
                myFaction.gold -= selectedUnit.goldCost;
                myFaction.wood -= selectedUnit.woodCost;

                GetRegion().GetComponent<RegionHandler>().isCreating = true;

                //createUnit and send request
                ServerResponse response = new ServerResponse();
                response.EventType = "CreateUnit";
                response.data.Add(InfoManager.Instance.factionsManager.myFaction);
                response.data.Add(checkArmy.GetComponent<CheckArmy>().army.name);
                response.data.Add(selectedUnit.name);
                response.data.Add(InfoManager.Instance.selectedRegion);
                GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                parentT3Panel.SetActive(false);
            }
            else
            {
                toastManager.toasts.Enqueue("Don't have enough resources");
            }
        }
    }

    private GameObject GetRegion()
    {
        MapManager map = new MapManager();
        GameObject region = null;

        foreach (GameObject obj in map.GetRegions())
        {
            if (obj.GetComponent<RegionHandler>().region.name == InfoManager.Instance.selectedRegion)
            {
                region = obj;
            }
        }
        return region;
    }
}
