using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    private static InfoManager _instance;
    public static InfoManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("The InfoManager is null");
            }
            return _instance;
        }
    }

    public ArmyCollections armyCollections;
    public FactionsManager factionsManager;

    public string selectedRegion;
    public string destinyRegion;
    public bool canMoveArmy;
    public bool updating;
    public bool hasGameEnded;

    private void Awake()
    {
        armyCollections = new ArmyCollections();
        factionsManager = new FactionsManager();
        _instance = this;
        destinyRegion = "";
        canMoveArmy = true;
        updating = true;
        hasGameEnded = false;
    }
}
