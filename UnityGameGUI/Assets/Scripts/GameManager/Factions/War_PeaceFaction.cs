using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class War_PeaceFaction : MonoBehaviour
{
    private List<GameObject> factionsButtons;
    public GameObject cancelButton;
    public GameObject optionsPanel;
    public Text title;

    public bool declareWar;

    public void Awake()
    {
        factionsButtons = new List<GameObject>();
        GameObject buttonslist = transform.Find("Options").gameObject;
        buttonslist.transform.Find("Faction1").gameObject.SetActive(false);
        buttonslist.transform.Find("Faction2").gameObject.SetActive(false);
        buttonslist.transform.Find("Faction3").gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        factionsButtons.Clear();
        GameObject buttonslist = transform.Find("Options").gameObject;
        factionsButtons.Add(buttonslist.transform.Find("Faction1").gameObject);
        factionsButtons.Add(buttonslist.transform.Find("Faction2").gameObject);
        factionsButtons.Add(buttonslist.transform.Find("Faction3").gameObject);
        cancelButton.SetActive(true);
        setButtonsText();
        showButtons();
    }

    private void OnDisable()
    {
        foreach (GameObject button in factionsButtons)
        {
            button.SetActive(false);
        }
        cancelButton.SetActive(false);
    }

    public void ButtonAction(Button button)
    {
        string faction = button.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        Debug.Log(faction);
        Faction myFaction = Utils.GetMyFaction();
        if (declareWar)
        {
            myFaction.warWith[faction]["CanAttack"] = true;
            myFaction.warWith[faction]["DeclaredThisRound"] = true;

            ServerResponse response = new ServerResponse();
            response.EventType = "DeclareWar";
            response.data.Add(myFaction.name);
            response.data.Add(faction);
            GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));
        }
        else
        {
            myFaction.warWith[faction]["CanAttack"] = false;
            myFaction.warWith[faction]["DeclaredThisRound"] = true;
            myFaction.armistice = true;

            ServerResponse response = new ServerResponse();
            response.EventType = "Armistice";
            response.data.Add(myFaction.name);
            response.data.Add(faction);
            GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));
        }
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        this.transform.parent.gameObject.SetActive(false);
        optionsPanel.SetActive(true);
    }

    private void showButtons()
    {
        List<string> warFactions = null;
        if (declareWar)
        {
            warFactions = GetAttackFactions();
        }
        else
        {
            warFactions = GetInWarFactions();
        }
        foreach (string faction in warFactions)
        {
            foreach (GameObject button in factionsButtons)
            {
                if (faction == button.transform.Find("Text").gameObject.GetComponent<Text>().text)
                {
                    button.SetActive(true);
                }
            }
        }
    }

    private void setButtonsText()
    {
        if (declareWar)
        {
            title.text = "Declare War To";
        }
        else
        {
            title.text = "Make Peace With";
        }

        Faction myFaction = Utils.GetMyFaction();

        int i = 0;
        foreach (var entry in myFaction.warWith)
        {
            factionsButtons[i].transform.Find("Text").gameObject.GetComponent<Text>().text = entry.Key;
            i++;
        }
    }

    private List<string> GetInWarFactions()
    {
        List<string> inWarFactions = new List<string>();
        Faction myFaction = Utils.GetMyFaction();

        foreach (var entry in myFaction.warWith)
        {
            if (entry.Value["CanAttack"] == true && entry.Value["DeclaredThisRound"] == false && entry.Value["AttackedThisRound"] == false)
            {
                inWarFactions.Add(entry.Key);
            }
        }

        return inWarFactions;
    }

    private List<string> GetAttackFactions()
    {
        List<string> factions = new List<string>();

        MapManager map = new MapManager();
        GameObject myRegion = Utils.GetRegion(InfoManager.Instance.selectedRegion);
        Faction myFaction = Utils.GetMyFaction();

        string[] borders = myRegion.GetComponent<RegionHandler>().region.borders.ToArray();

        foreach (string border in borders)
        {
            RegionHandler destiny = Utils.GetRegion(border).GetComponent<RegionHandler>();
            
            if (destiny.region.owner != InfoManager.Instance.factionsManager.myFaction &&
                destiny.region.owner != "Neutral")
            {
                foreach (var entry in myFaction.warWith)
                {
                    if (entry.Key == destiny.region.owner)
                    {
                        if (entry.Value["CanAttack"] == false && entry.Value["AttackedThisRound"] == false)
                        {
                            if (!factions.Any(p => p == destiny.region.owner))
                            {
                                factions.Add(destiny.region.owner);
                            }
                        }
                    }
                }
            }
        }
        return factions;
    }
}
