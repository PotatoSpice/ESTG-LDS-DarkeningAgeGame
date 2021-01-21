using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public string type;
    public General general;
    public Unit unit;
    private List<General> optionsGeneralList;
    private List<Unit> optionsUnitList;
    public List<GameObject> prefabsList;
    public GameObject prefab;

    private void Awake()
    {
        optionsGeneralList = new List<General>();
        optionsUnitList = new List<Unit>();
        prefabsList = new List<GameObject>();
    }

    private void OnEnable()
    {
        general = null;
        unit = null;
        GetCharacters();
    }

    private void OnDisable()
    {
        foreach (GameObject obj in prefabsList)
        {
            Destroy(obj);
        }
    }

    private void GetCharacters()
    {
        optionsGeneralList.Clear();
        optionsUnitList.Clear();
        prefabsList.Clear();
        if (type == "General")
        {
            foreach (General general in InfoManager.Instance.armyCollections.getGeneral())
            {
                if (general.faction == InfoManager.Instance.factionsManager.myFaction && general.available == true)
                {
                    GameObject newObj;
                    newObj = (Instantiate(prefab, transform));
                    newObj.GetComponent<Character>().type = "General";
                    newObj.GetComponent<Character>().general = general;
                    prefabsList.Add(newObj);
                    optionsGeneralList.Add(general);
                }
            }
        }
        else
        {
            foreach (Unit unit in InfoManager.Instance.armyCollections.getUnits())
            {
                GameObject newObj;
                newObj = (Instantiate(prefab, transform));
                newObj.GetComponent<Character>().type = "Unit";
                newObj.GetComponent<Character>().unit = unit;
                prefabsList.Add(newObj);
                optionsUnitList.Add(unit);
            }
        }
    }
}
