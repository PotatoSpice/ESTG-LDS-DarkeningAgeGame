using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RegionOptions : MonoBehaviour
{
    private string regionName;
    private MapManager mapManager;

    //Army GameObjects
    public GameObject createArmyInvisible;

    public GameObject createArmyPanel;
    public GameObject createArmyButton;

    public GameObject checkArmyPanel;
    public GameObject checkArmyButton;

    public GameObject moveArmyButton;
    public GameObject swapArmyButton;
    public GameObject attackButton;

    public GameObject declareWarButton;
    public GameObject declarePeaceButton;
    public GameObject declareWar_PeacePanel;

    public GameObject toastObj;
    private Toast toastManager;

    private void OnEnable()
    {
        mapManager = new MapManager();
        regionName = InfoManager.Instance.selectedRegion;
        DisableButtons();
        showButtons();
        toastManager = toastObj.GetComponent<Toast>();
    }

    private void OnDisable()
    {
        InfoManager.Instance.canMoveArmy = true;
    }

    private void Update()
    {
        if (regionName != InfoManager.Instance.selectedRegion)
        {
            mapManager = new MapManager();
            regionName = InfoManager.Instance.selectedRegion;
            DisableButtons();
            showButtons();
        }
    }

    public void CreateArmy()
    {
        transform.parent.gameObject.SetActive(false);
        createArmyInvisible.SetActive(true);
        createArmyPanel.SetActive(true);
        createArmyPanel.GetComponent<CreateArmy>().region = regionName;
    }

    public void checkArmy()
    {
        transform.parent.gameObject.SetActive(false);
        createArmyInvisible.SetActive(true);
        checkArmyPanel.SetActive(true);
    }

    public async void moveArmy()
    {
        InfoManager.Instance.canMoveArmy = false;
        GameObject region = Utils.GetRegion(regionName);

        //zero quando o destino nao tem exercitos
        int canMove = 0;
        GameObject destinyRegion = null;
        string destiny = "";
        await Task.Run(() => WaitForValue());
        destiny = InfoManager.Instance.destinyRegion;
        destinyRegion = Utils.GetRegion(destiny);

        if (destiny != "")
        {
            if (region.GetComponent<RegionHandler>().region.borders.Contains(destiny))
            {
                //verificar se o destino tem um exercito presente e pode mover para la
                foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
                {
                    foreach (Army army in fac.armies)
                    {
                        if (army.region == destiny && destinyRegion.GetComponent<RegionHandler>().canMoveArmy == false)
                        {
                            canMove += 1;
                        }
                    }
                }

                if (destinyRegion.GetComponent<RegionHandler>().region.owner == InfoManager.Instance.factionsManager.myFaction && canMove == 0)
                {
                    string army = "";
                    foreach (Army ar in GetMyArmies())
                    {
                        if (ar.region == regionName)
                        {
                            army = ar.name;
                        }
                    }

                    ServerResponse response = new ServerResponse();
                    response.EventType = "MoveArmy";
                    response.data.Add(army);
                    response.data.Add(destiny);
                    response.data.Add(InfoManager.Instance.factionsManager.myFaction);
                    GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                    toastManager.toasts.Enqueue("Moving army.");
                    transform.parent.gameObject.SetActive(false);

                    destinyRegion.GetComponent<RegionHandler>().canMoveArmy = false;
                    destinyRegion.GetComponent<RegionHandler>().canCreateArmy = false;
                    region.GetComponent<RegionHandler>().isMoving = true;

                    InfoManager.Instance.destinyRegion = "";
                }
                else if (destinyRegion.GetComponent<RegionHandler>().region.owner == "Neutral" && canMove == 0)
                {
                    string army = "";
                    foreach (Army ar in GetMyArmies())
                    {
                        if (ar.region == regionName)
                        {
                            army = ar.name;
                        }
                    }
                    ServerResponse response = new ServerResponse();
                    response.EventType = "AnnexNeutralRegion";
                    response.data.Add(army);
                    response.data.Add(destiny);
                    response.data.Add(regionName);
                    GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                    toastManager.toasts.Enqueue("Moving army.");
                    transform.parent.gameObject.SetActive(false);

                    destinyRegion.GetComponent<RegionHandler>().canMoveArmy = false;
                    region.GetComponent<RegionHandler>().isMoving = true;

                    InfoManager.Instance.destinyRegion = "";
                }
                else if (destinyRegion.GetComponent<RegionHandler>().region.owner != "Neutral" && canMove == 0 &&
                    destinyRegion.GetComponent<RegionHandler>().region.owner != InfoManager.Instance.factionsManager.myFaction)
                {
                    bool value = false;
                    Faction myFaction = Utils.GetMyFaction();
                    foreach (var entry in myFaction.warWith)
                    {
                        if (entry.Key == destinyRegion.GetComponent<RegionHandler>().region.owner)
                        {
                            if (entry.Value["CanAttack"] == true && entry.Value["AttackedThisRound"] == false && entry.Value["DeclaredThisRound"] == false)
                            {
                                value = true;
                            }
                        }
                    }
                    if (value)
                    {
                        string army = "";
                        foreach (Army ar in GetMyArmies())
                        {
                            if (ar.region == regionName)
                            {
                                army = ar.name;
                            }
                        }

                        ServerResponse response = new ServerResponse();
                        response.EventType = "AttackRegion";
                        response.data.Add(regionName);
                        response.data.Add(destiny);
                        response.data.Add(army);
                        response.data.Add("");
                        GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                        toastManager.toasts.Enqueue("Moving army.");
                        transform.parent.gameObject.SetActive(false);

                        destinyRegion.GetComponent<RegionHandler>().canMoveArmy = false;
                        region.GetComponent<RegionHandler>().isMoving = true;

                        InfoManager.Instance.destinyRegion = "";
                    }
                    else
                    {
                        toastManager.toasts.Enqueue("You need to declare war.");
                        Debug.Log("You need to declare war.");
                    }
                }
                else
                {
                    toastManager.toasts.Enqueue("Cant move to that region.");
                    Debug.Log("Cant move to that region.");
                }
            }
            else
            {
                toastManager.toasts.Enqueue("Cant move to that region.");
                Debug.Log("Cant move to that region");
                InfoManager.Instance.destinyRegion = "";
            }
        }
        InfoManager.Instance.canMoveArmy = true;
    }

    public async void SwapArmy()
    {
        InfoManager.Instance.canMoveArmy = false;
        GameObject region = Utils.GetRegion(regionName);

        int canMove = 0;
        GameObject destinyRegion = null;
        string destiny = "";
        await Task.Run(() => WaitForValue());
        destiny = InfoManager.Instance.destinyRegion;
        destinyRegion = Utils.GetRegion(destiny);

        if (destiny != "")
        {
            if (region.GetComponent<RegionHandler>().region.borders.Contains(destiny))
            {
                foreach (Army ar in GetMyArmies())
                {
                    //se o destino tem um exercito
                    if (destiny == ar.region)
                    {
                        canMove += 1;
                    }
                }
                if (canMove == 1)
                {
                    string army = "";
                    foreach (Army ar in GetMyArmies())
                    {
                        if (ar.region == regionName)
                        {
                            army = ar.name;
                        }
                    }
                    ServerResponse response = new ServerResponse();
                    response.EventType = "SwapArmies";
                    response.data.Add(army);
                    response.data.Add(destiny);
                    response.data.Add(InfoManager.Instance.factionsManager.myFaction);
                    GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                    toastManager.toasts.Enqueue("Swapping armies.");
                    transform.parent.gameObject.SetActive(false);

                    destinyRegion.GetComponent<RegionHandler>().isMoving = true;
                    region.GetComponent<RegionHandler>().isMoving = true;

                    InfoManager.Instance.destinyRegion = "";
                }
                else
                {
                    Debug.Log("Cant move to that region.");
                    toastManager.toasts.Enqueue("Cant move to that region.");
                    InfoManager.Instance.destinyRegion = "";
                }
            }
        }
        InfoManager.Instance.canMoveArmy = true;
    }

    public async void AttackEnemy()
    {
        InfoManager.Instance.canMoveArmy = false;
        GameObject region = Utils.GetRegion(regionName);
        Army destinyArmy = null;

        //zero quando o destino nao tem exercitos

        GameObject destinyRegion = null;
        string destiny = "";
        await Task.Run(() => WaitForValue());
        destiny = InfoManager.Instance.destinyRegion;
        destinyRegion = Utils.GetRegion(destiny);

        if (destiny != "")
        {
            if (region.GetComponent<RegionHandler>().region.borders.Contains(destiny))
            {
                //verificar se o destino tem um exercito presente e pode mover para la
                foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
                {
                    if (fac.name != InfoManager.Instance.factionsManager.myFaction)
                    {
                        foreach (Army army in fac.armies)
                        {
                            if (army.region == destiny)
                            {
                                if (!CheckWarDeclaredThisRound(fac.name))
                                {
                                    destinyArmy = army;
                                }
                            }
                        }
                    }
                }
                if (destinyRegion.GetComponent<RegionHandler>().region.owner != "Neutral" && destinyArmy != null &&
                    destinyRegion.GetComponent<RegionHandler>().region.owner != InfoManager.Instance.factionsManager.myFaction)
                {
                    string army = "";
                    foreach (Army ar in GetMyArmies())
                    {
                        if (ar.region == regionName)
                        {
                            army = ar.name;
                            ar.attackRound = true;
                        }
                    }

                    ServerResponse response = new ServerResponse();
                    response.EventType = "AttackRegion";
                    response.data.Add(regionName);
                    response.data.Add(destiny);
                    response.data.Add(army);
                    response.data.Add(destinyArmy.name);
                    GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));

                    toastManager.toasts.Enqueue("Attacking Enemy.");
                    transform.parent.gameObject.SetActive(false);

                    Faction myFaction = Utils.GetMyFaction();
                    myFaction.warWith[destinyRegion.GetComponent<RegionHandler>().region.owner]["AttackedThisRound"] = true;

                    region.GetComponent<RegionHandler>().isMoving = true;
                }
                else
                {
                    toastManager.toasts.Enqueue("Can't move to that region");
                    Debug.Log("Region does't belong to the enemy or doesn't have an army.");
                }
            }
            else
            {
                toastManager.toasts.Enqueue("Can't move to that region");
                Debug.Log("Cant move to that region");
            }
        }
        InfoManager.Instance.destinyRegion = "";
        InfoManager.Instance.canMoveArmy = true;
    }

    public void DeclareWar()
    {
        transform.parent.gameObject.SetActive(false);
        declareWar_PeacePanel.SetActive(true);
        declareWar_PeacePanel.transform.Find("FactionOptions").gameObject.GetComponent<War_PeaceFaction>().declareWar = true;
        declareWar_PeacePanel.transform.Find("FactionOptions").gameObject.SetActive(true);
    }

    public void DeclarePeace()
    {
        transform.parent.gameObject.SetActive(false);
        declareWar_PeacePanel.SetActive(true);
        declareWar_PeacePanel.transform.Find("FactionOptions").gameObject.GetComponent<War_PeaceFaction>().declareWar = false;
        declareWar_PeacePanel.transform.Find("FactionOptions").gameObject.SetActive(true);
    }

    private void showButtons()
    {
        bool regionHasArmy = false;//region has own army
        bool canShowCreateArmy = false;
        bool canShowCheckArmy = false;
        bool canMoveArmy = false; //check if as own army
        bool canSwapArmy = false; //check if regions are adjacent and both have armies
        bool canAttack = false; //check if can attack

        Region region = Utils.GetRegion(regionName).GetComponent<RegionHandler>().region;

        if (region.type.type == "Urban" || region.type.type == "Fort" || region.type.type == "Capital" || region.type.type == "SpecialCapital")
        {
            if (region.owner == InfoManager.Instance.factionsManager.myFaction)
            {
                canShowCreateArmy = true;
            }
        }

        foreach (Army ar in GetMyArmies())
        {
            if (ar.region == region.name)
            {
                regionHasArmy = true;
            }
        }

        if (regionHasArmy)
        {
            canShowCreateArmy = false;
            canShowCheckArmy = true;
            canMoveArmy = true;
            canAttack = true;
        }

        //verificar se pode criar na propria regiao
        if (Utils.GetRegion(regionName).GetComponent<RegionHandler>().canCreateArmy == false)
        {
            canShowCreateArmy = false;
        }

        if (CheckIfArmiesOnBorders())
        {
            canSwapArmy = true;
        }

        if (!regionHasArmy)
        {
            canSwapArmy = false;
        }

        //verificar se pode deslocar
        if (Utils.GetRegion(regionName).GetComponent<RegionHandler>().isMoving == true)
        {
            canMoveArmy = false;
            canSwapArmy = false;
            canAttack = false;
        }

        //verificar se acabou de ser criado
        if (Utils.GetRegion(regionName).GetComponent<RegionHandler>().isCreating == true)
        {
            canMoveArmy = false;
            canSwapArmy = false;
            canAttack = false;
        }

        if (!CheckGeneralsAvailable())
        {
            canShowCreateArmy = false;
        }


        if (CheckIfEnemiesOnBorders(true) && InfoManager.Instance.updating == false)
        {
            if (canAttack)
            {
                attackButton.SetActive(true);
            }
        }
        if (CheckIfWarWithAll() && InfoManager.Instance.updating == false)
        {
            declareWarButton.SetActive(true);
        }
        if (CheckIfWarWithFactions() && InfoManager.Instance.updating == false && Utils.GetMyFaction().armistice == false)
        {
            declarePeaceButton.SetActive(true);
        }
        if (canShowCheckArmy)
        {
            checkArmyButton.SetActive(true);
        }
        if (canShowCreateArmy && InfoManager.Instance.updating == false)
        {
            createArmyButton.SetActive(true);
        }
        if (canSwapArmy && InfoManager.Instance.updating == false)
        {
            swapArmyButton.SetActive(true);
        }
        if (canMoveArmy && InfoManager.Instance.updating == false)
        {
            moveArmyButton.SetActive(true);
        }
    }

    private void DisableButtons()
    {
        createArmyButton.SetActive(false);
        checkArmyButton.SetActive(false);
        moveArmyButton.SetActive(false);
        swapArmyButton.SetActive(false);
        attackButton.SetActive(false);
        declareWarButton.SetActive(false);
        declarePeaceButton.SetActive(false);
    }

    /// <summary>
    /// Check if my faction has any army in the borders of the selected region
    /// </summary>
    /// <returns>true if has any army that isn't moving or was created in the round</returns>
    private bool CheckIfArmiesOnBorders()
    {
        string[] borders = Utils.GetRegion(regionName).GetComponent<RegionHandler>().region.borders.ToArray();

        foreach (Army ar in GetMyArmies())
        {
            foreach (string border in borders)
            {
                if (border == ar.region)
                {
                    foreach (GameObject reg in mapManager.GetRegions())
                    {
                        if (reg.GetComponent<RegionHandler>().region.name == border)
                        {
                            if (reg.GetComponent<RegionHandler>().isCreating == false && reg.GetComponent<RegionHandler>().isMoving == false)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check if my faction has generals available
    /// </summary>
    /// <returns>true if generals available</returns>
    private bool CheckGeneralsAvailable()
    {
        foreach (General gen in InfoManager.Instance.armyCollections.getGeneral())
        {
            if (gen.faction == InfoManager.Instance.factionsManager.myFaction && gen.available == true)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if exists enemy armies on borders and if can attack them
    /// </summary>
    /// <param name="wantToAttack"></param>if true want's to attack else want's to declare war
    /// <returns>true if can attack else return false</returns>
    private bool CheckIfEnemiesOnBorders(bool wantToAttack)
    {
        string[] borders = Utils.GetRegion(regionName).GetComponent<RegionHandler>().region.borders.ToArray();

        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            if (fac.name != InfoManager.Instance.factionsManager.myFaction)
            {
                if (CheckIfWarWasDeclared(fac.name) == wantToAttack)
                {
                    foreach (Army ar in fac.armies)
                    {
                        foreach (string border in borders)
                        {
                            if (border == ar.region)
                            {
                                foreach (GameObject reg in mapManager.GetRegions())
                                {
                                    if (reg.GetComponent<RegionHandler>().region.name == border)
                                    {
                                        if (!CheckWarDeclaredThisRound(fac.name))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check if war was already declared with faction
    /// </summary>
    /// <param name="faction">name of the faction</param>
    /// <returns>true if war was already declared</returns>
    private bool CheckIfWarWasDeclared(string faction)
    {
        bool returnValue;
        Dictionary<string, bool> inWar;
        Faction myFaction = Utils.GetMyFaction();

        myFaction.warWith.TryGetValue(faction, out inWar);

        inWar.TryGetValue("CanAttack", out returnValue);
        return returnValue;
    }

    /// <summary>
    /// Check if war with faction was declared this round
    /// </summary>
    /// <param name="faction">name of the faction</param>
    /// <returns>false if war was't declared this round</returns>
    private bool CheckWarDeclaredThisRound(string faction)
    {
        bool returnValue;
        Dictionary<string, bool> inWar;
        Faction myFaction = Utils.GetMyFaction();

        myFaction.warWith.TryGetValue(faction, out inWar);

        inWar.TryGetValue("DeclaredThisRound", out returnValue);
        return returnValue;
    }

    /// <summary>
    /// Check if in war with some faction
    /// </summary>
    /// <returns>true if is in war</returns>
    private bool CheckIfWarWithFactions()
    {
        Faction myFaction = Utils.GetMyFaction();
        foreach (var entry in myFaction.warWith)
        {
            if (entry.Value["CanAttack"] == true && entry.Value["AttackedThisRound"] == false && entry.Value["DeclaredThisRound"] == false)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfWarWithAll()
    {
        bool value = false;
        Region region = Utils.GetRegion(regionName).GetComponent<RegionHandler>().region;

        if(region.owner == InfoManager.Instance.factionsManager.myFaction)
        {
            foreach (string border in region.borders)
            {
                Region destiny = Utils.GetRegion(border).GetComponent<RegionHandler>().region;
                if (destiny.owner != InfoManager.Instance.factionsManager.myFaction &&
                    destiny.owner != "Neutral")
                {
                    Faction myFaction = Utils.GetMyFaction();
                    foreach (var entry in myFaction.warWith)
                    {
                        if (entry.Key == destiny.owner)
                        {
                            if (entry.Value["CanAttack"] == false && entry.Value["AttackedThisRound"] == false && entry.Value["DeclaredThisRound"] == false)
                            {
                                value = true;
                            }
                        }
                    }
                }
            }
        }
        return value;
    }

    private List<Army> GetMyArmies()
    {
        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            if (fac.name == InfoManager.Instance.factionsManager.myFaction)
            {
                return fac.armies;
            }
        }
        return null;
    }

    private void WaitForValue()
    {
        while (InfoManager.Instance.destinyRegion == "" && !InfoManager.Instance.canMoveArmy)
        {

        }
    }
}
