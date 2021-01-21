using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateArmy : MonoBehaviour
{
    private GameObject generalPrefab;
    private GameObject unitPrefab;
    public GameObject setUnitsPanel;
    public GameObject prefab;
    public GameObject createChar;
    public InputField input;

    public Text goldCosts;
    public Text woodCosts;
    public Text foodCosts;
    private int goldCostsInt;
    private int woodCostsInt;
    private int foodCostsInt;

    private General general;
    private Unit unit;
    public string region;
    private MapManager mapManager;

    public GameObject toastObj;

    void Start()
    {
        mapManager = new MapManager();

        for (int i = 0; i < 2; i++)
        {
            GameObject newObj;
            newObj = (Instantiate(prefab, setUnitsPanel.transform));
            if (i == 0)
            {
                newObj.GetComponent<SelectPrefab>().type = "General";
                generalPrefab = newObj;
            }
            if (i == 1)
            {
                newObj.GetComponent<SelectPrefab>().type = "Unit";
                unitPrefab = newObj;
            }
        }
        createChar.SetActive(false);
        generalPrefab.SetActive(true);
        unitPrefab.SetActive(true);
    }

    private void Update()
    {
        if(unitPrefab.GetComponent<SelectPrefab>().unit != null && generalPrefab.GetComponent<SelectPrefab>().general != null)
        {
            goldCostsInt = generalPrefab.GetComponent<SelectPrefab>().general.goldCost + unitPrefab.GetComponent<SelectPrefab>().unit.goldCost;
            goldCosts.text = "Gold: " + goldCostsInt;
            foodCostsInt = unitPrefab.GetComponent<SelectPrefab>().unit.foodCost;
            foodCosts.text = "Food: " + foodCostsInt;
            woodCostsInt = unitPrefab.GetComponent<SelectPrefab>().unit.woodCost;
            woodCosts.text = "Wood: " + woodCostsInt;
        }
    }

    private void OnDisable()
    {
        ResetValues();
    }

    public void CloseCreateChar()
    {
        createChar.SetActive(false);
    }

    public void CreateArmyRequest()
    {
        string textValue = input.text;
        unit = unitPrefab.GetComponent<SelectPrefab>().unit;
        general = generalPrefab.GetComponent<SelectPrefab>().general;
        region = InfoManager.Instance.selectedRegion;
        string factionName = InfoManager.Instance.factionsManager.myFaction;

        Toast toastManager = toastObj.GetComponent<Toast>();

        if (unit != null && general.name != null && !string.IsNullOrWhiteSpace(textValue))
        {
            bool validName = true;
            foreach(Faction fac in InfoManager.Instance.factionsManager.factions.ToList())
            {
                foreach (Army ar in fac.armies)
                {
                    if(ar.name == textValue)
                    {
                        validName = false;
                    }
                }
            }
            if (validName)
            {
                Faction faction = Utils.GetMyFaction();
                if (goldCostsInt < faction.gold && foodCostsInt < faction.food && woodCostsInt < faction.wood)
                {
                    ServerResponse response = new ServerResponse();
                    response.EventType = "CreateArmy";
                    response.data.Add(factionName);
                    response.data.Add(general.name);
                    response.data.Add(textValue);
                    response.data.Add(region);
                    response.data.Add(unit.name);

                    faction.gold -= goldCostsInt;
                    faction.food -= foodCostsInt;
                    faction.wood -= woodCostsInt;

                    Army army = new Army();
                    army.name = textValue;
                    army.general = general;
                    foreach (General gen in InfoManager.Instance.armyCollections.getGeneral())
                    {
                        if(gen.name == general.name)
                        {
                            gen.available = false;
                        }
                    }
                    UnitArmy unitArmy = new UnitArmy();
                    unitArmy.unit = unit;
                    unitArmy.availableManPower = unit.maxManpower;
                    unitArmy.reinforcementCosts = 0;
                    army.units.Add(unitArmy);
                    army.region = region;
                    army.unitCount = 1;
                    army.totalUnitCount = 1;

                    Utils.GetMyFaction().armies.Add(army);

                    GameObject regionHandler = GetCurrentRegion();
                    regionHandler.GetComponent<RegionHandler>().canCreateArmy = false;
                    regionHandler.GetComponent<RegionHandler>().canMoveArmy = false;
                    regionHandler.GetComponent<RegionHandler>().isMoving = true;
                    regionHandler.GetComponent<RegionHandler>().createdThisRound = true;
                    regionHandler.GetComponent<RegionHandler>().armyImage.SetActive(true);

                    GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));
                    ResetValues();

                    this.gameObject.SetActive(false);
                    this.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    toastManager.toasts.Enqueue("Don't have enough resources");
                    Debug.Log("Dont have enough resources");
                }
            }
            else
            {
                toastManager.toasts.Enqueue("Choose other name");
                Debug.Log("Choose other name.");
            }
        }
    }

    private void ResetValues()
    {
        general = null;
        unit = null;
        unitPrefab.GetComponent<SelectPrefab>().unit = null;
        generalPrefab.GetComponent<SelectPrefab>().general = null;
        input.text = "";
        goldCosts.text = "";
        foodCosts.text = "";
        woodCosts.text = "";
    }

    private GameObject GetCurrentRegion()
    {
        List<GameObject> regionsObj = mapManager.GetRegions();
        foreach (GameObject reg in regionsObj)
        {
            if (reg.GetComponent<RegionHandler>().region.name == region)
            {
                return reg;
            }
        }
        return null;
    }
}
