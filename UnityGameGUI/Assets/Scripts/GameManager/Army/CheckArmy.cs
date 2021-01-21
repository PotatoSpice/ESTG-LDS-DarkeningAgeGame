using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckArmy : MonoBehaviour
{
    //public GameObject addUnits;

    public GameObject generalPanel;
    private GameObject general;
    public GameObject prefabGeneral;

    public GameObject armyUnitsPanel;
    private List<GameObject> armyUnitsObjects;
    public GameObject prefabUnit;

    public GameObject addUnitPanel;

    public Text armyInfo;

    public Army army;
    private int unitsCount;
    private string region;

    private void Awake()
    {
        armyUnitsObjects = new List<GameObject>();
    }

    private void OnEnable()
    {
        region = InfoManager.Instance.selectedRegion;
        SetArmyInfo();
        SetTextValue();
        SetPrefabs();
    }

    private void Update()
    {
        if (army != null)
        {
            SetArmyInfo();
            if (unitsCount != army.units.Count)
            {
                Clear();
                SetPrefabs();
                unitsCount = army.units.Count;
            }
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    private void SetArmyInfo()
    {
        Faction faction = GetMyFaction();

        foreach (Army exercito in faction.armies)
        {
            if (exercito.region == region)
            {
                this.army = exercito;
            }
        }
    }

    private Faction GetMyFaction()
    {
        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            if (fac.name == InfoManager.Instance.factionsManager.myFaction)
            {
                return fac;
            }
        }
        return null;
    }

    private void SetPrefabs()
    {
        general = (Instantiate(prefabGeneral, generalPanel.transform));
        general.GetComponent<Character>().type = "General";
        general.GetComponent<Character>().general = army.general;

        foreach (UnitArmy unit in army.units)
        {
            GameObject obj;
            obj = (Instantiate(prefabUnit, armyUnitsPanel.transform));
            obj.GetComponent<UpgradableChar>().unit = unit;
            obj.GetComponent<UpgradableChar>().army = army.name;
            armyUnitsObjects.Add(obj);
        }
        RegionHandler myRegion = GetRegion();

        if (army.units.Count == 15)
        {
            addUnitPanel.SetActive(false);
        }
        else if (InfoManager.Instance.updating == true)
        {
            addUnitPanel.SetActive(false);
        }
        else if (myRegion.region.type.type != "Urban" && myRegion.region.type.type != "Fort" 
            && myRegion.region.type.type != "Capital" && myRegion.region.type.type != "SpecialCapital")
        {
            addUnitPanel.SetActive(false);
        }
        else if (myRegion.isMoving && !myRegion.createdThisRound)
        {
            addUnitPanel.SetActive(false);
        }
        else
        {
            addUnitPanel.SetActive(true);
        }
    }

    private void SetTextValue()
    {
        armyInfo.text = "<b>Name: </b>" + army.name + "\n" +
                "<b>Unit Count: </b>" + army.unitCount + "\n" +
                "<b>Total Unit Count: </b>" + army.totalUnitCount + "\n" +
                "<b>CombatLock: </b>" + army.combatLock + "\n" +
                "<b>Gold Maintenance: </b>" + army.goldMaintenance + "\n" +
                "<b>Food Maintenance: </b>" + army.foodMaintenance;
    }

    private void Clear()
    {
        Destroy(general);

        foreach (GameObject obj in armyUnitsObjects)
        {
            Destroy(obj);
        }
        armyUnitsObjects.Clear();
    }

    private RegionHandler GetRegion()
    {
        MapManager mapManager = new MapManager();
        List<GameObject> regionsObj = mapManager.GetRegions();
        foreach (GameObject reg in regionsObj)
        {
            if (reg.GetComponent<RegionHandler>().region.name == army.region)
            {
                return reg.GetComponent<RegionHandler>();
            }
        }
        return null;
    }
}
