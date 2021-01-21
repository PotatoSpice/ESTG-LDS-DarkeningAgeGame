using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Assets.Scripts.Utils;
using System.Threading.Tasks;
using TMPro;

public class GameHandler : MonoBehaviour
{
    private string userToken;
    private string userID;
    private string roomID;
    private bool connected;
    private bool startGame;

    //Chat Variables
    public ChatManager chat;
    public Text chatText;
    public InputField chatInput;

    public GameObject canvasManager;

    public GameObject showResources;

    public GameObject optionsPanel;
    public GameObject giveUpButton;

    public GameObject clock;

    public GameObject winScreen;
    public GameObject KeepWatchingButton;
    public Faction myFaction = null;

    public GameObject loadingScreen;
    public GameObject optionsButton;

    public GameObject roundUpdatesPanel;
    public Text roundUpdatesText;
    public Text winConditionText;

    public TextMeshProUGUI scrollFaction;

    private MapManager map;

    void Start()
    {
        userToken = "";
        userID = "";
        roomID = "";
        roundUpdatesText.text = "";
        loadingScreen.SetActive(true);
        roundUpdatesPanel.SetActive(false);
        winScreen.SetActive(false);
        connected = false;
        startGame = false;

        chat = new ChatManager(chatText, chatInput);
        map = new MapManager();

        Task task = GetArgumentsValues();
        ConnectToServer();
    }

    private void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    private void Update()
    {
        if (connected == true)
        {
            var receiveQueue = GameCommunication.Instance.connection.receiveQueue;
            string serverMsg = "";
            while (receiveQueue.TryPeek(out serverMsg))
            {
                receiveQueue.TryDequeue(out serverMsg);
                HandleMessage(serverMsg);
            }
            if (!InfoManager.Instance.hasGameEnded)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (optionsPanel.activeSelf)
                    {
                        if (Utils.GetMyFaction().defeated)
                        {
                            giveUpButton.SetActive(false);
                        }
                        else
                        {
                            optionsPanel.SetActive(false);
                            optionsButton.SetActive(true);
                        }
                    }
                    else
                    {
                        optionsPanel.SetActive(true);
                        optionsButton.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    if (roundUpdatesPanel.activeSelf)
                    {
                        roundUpdatesPanel.SetActive(false);
                    }
                    else
                    {
                        if (!Utils.GetMyFaction().defeated && !optionsPanel.activeSelf && !InfoManager.Instance.hasGameEnded && chatInput.text == "")
                        {
                            roundUpdatesPanel.SetActive(true);
                        }
                    }
                }
                if (InfoManager.Instance.updating)
                {
                    giveUpButton.SetActive(false);
                }
                else
                {
                    if (!Utils.GetMyFaction().defeated)
                    {
                        giveUpButton.SetActive(true);
                    }
                    else
                    {
                        giveUpButton.SetActive(false);
                    }
                }
            }
            if (myFaction != null)
            {
                this.checkWin();
                this.CheckLose();
            }
        }
    }

    //Method process information from server
    private void HandleMessage(string serverMsg)
    {
        JObject jsonResponse = new JObject();
        bool isResponse = false;

        try
        {
            object objResponse = JsonConvert.DeserializeObject(serverMsg);
            jsonResponse = JObject.Parse(objResponse.ToString());
            isResponse = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            isResponse = false;
        }

        if (isResponse)
        {
            var valueRaw = jsonResponse["EventType"];
            if(valueRaw == null)
            {
                valueRaw = jsonResponse["eventType"];
            }
            if (valueRaw == null)
            {
                valueRaw = jsonResponse["errorType"];
            }
            if (valueRaw != null)
            {
                switch (valueRaw.ToString())
                {
                    case "ChatMessage":
                        chat.AddMessageToHistory(jsonResponse["Username"].ToString(), jsonResponse["Message"].ToString());
                        break;
                    case "InitialInformation":
                        map.SetRegionsValues(jsonResponse);
                        InfoManager.Instance.armyCollections.SetUnitsCollection(jsonResponse);
                        InfoManager.Instance.armyCollections.SetGeneralsCollection(jsonResponse);
                        InfoManager.Instance.factionsManager.SetFactionInformation(jsonResponse, true);
                        InfoManager.Instance.factionsManager.SetFactionMaintenance(jsonResponse);
                        InfoManager.Instance.factionsManager.SetFactionsWarsStates(jsonResponse, false);
                        clock.GetComponent<StopWatch>().SetRound(jsonResponse);
                        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
                        {
                            if (fac.player == userID)
                            {
                                InfoManager.Instance.factionsManager.myFaction = fac.name;
                                scrollFaction.text = fac.name;
                                scrollFaction.color = SetScrollFactionColor(fac.name);
                            }
                        }

                        showResources.GetComponent<ShowResources>().UpdateResources();
                        myFaction = Utils.GetMyFaction();
                        if (clock.GetComponent<StopWatch>().roundNum == 0)
                        {
                            loadingScreen.SetActive(false);
                            startGame = true;
                        }
                        else
                        {
                            Text showFaction = loadingScreen.transform.Find("Faction").gameObject.GetComponent<Text>();
                            showFaction.text = "WAIT FOR END OF ROUND" + "\n" + showFaction.text;
                        }
                        this.WinConditionText();
                        break;
                    case "UpdateInformation":
                        roundUpdatesText.text = "";
                        canvasManager.GetComponent<PanelsManager>().CloseAllPanels();

                        clock.GetComponent<StopWatch>().SetRound(jsonResponse);

                        InfoManager.Instance.factionsManager.SetFactionInformation(jsonResponse, false);
                        InfoManager.Instance.factionsManager.SetFactionMaintenance(jsonResponse);
                        InfoManager.Instance.armyCollections.SetGeneralsCollection(jsonResponse);
                        InfoManager.Instance.factionsManager.AddArmies(jsonResponse);
                        InfoManager.Instance.factionsManager.SetFactionsWarsStates(jsonResponse, true);
                        DefineRegionAbleToCreateAndMove();
                        map.UpdateRegionsOwners(jsonResponse);
                        RoundUpdatesText();
                        if (!startGame)
                        {
                            loadingScreen.SetActive(false);
                            startGame = true;
                        }
                        break;
                    case "GameUpdatedConn":
                        ShowFaction(jsonResponse);
                        break;
                    case "RoomNotFoundException":
                        Task taskOne = CloseGame("Game not found");
                        break;
                    case "PlayerGameStarted":
                        Task taskTwo = CloseGame("Player already connected to game");
                        break;
                    default:
                        print("Response type not found.");
                        break;
                }
            }
        }
    }

    public void SendChatMessage()
    {
        string message = chat.SendChatMessage(userID);
        if (message != "")
        {
            GameCommunication.Instance.connection.SendMessage(message);
        }
    }

    private async void DisconnectFromServer()
    {
        if (connected)
        {
            await GameCommunication.Instance.connection.Disconnect();
        }
    }

    private void ConnectToServer()
    {
        GameCommunication.Instance.connection = new ClientConnection("ws://25.47.130.119:5000/ws-game/?room-id=" + roomID + "&player-auth=" + userToken);
        GameCommunication.Instance.connection.Connect();
        if (GameCommunication.Instance.connection.IsConnectionOpen())
        {
            connected = true;

            RequestInicialInfo();
        }
        else
        {
            Debug.Log("Couldn't connect.");
            Task taskOne = CloseGame("Couln't connect to server");
        }
    }

    private void RequestInicialInfo()
    {
        ServerResponse inicialRequest = new ServerResponse();
        inicialRequest.EventType = "InitialInformation";
        string inicialReqString = JsonConvert.SerializeObject(inicialRequest);
        GameCommunication.Instance.connection.SendMessage(inicialReqString);
    }

    private void DefineRegionAbleToCreateAndMove()
    {
        foreach (GameObject regionObj in map.GetRegions())
        {
            foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
            {
                foreach (Army ar in fac.armies)
                {
                    //Se tiver algum exercito
                    if (ar.region == regionObj.GetComponent<RegionHandler>().region.name)
                    {
                        regionObj.GetComponent<RegionHandler>().canCreateArmy = false;
                        regionObj.GetComponent<RegionHandler>().canMoveArmy = false;
                    }
                    else
                    {
                        //se a regiao for minha
                        if (regionObj.GetComponent<RegionHandler>().region.owner == InfoManager.Instance.factionsManager.myFaction)
                        {
                            regionObj.GetComponent<RegionHandler>().canCreateArmy = true;
                            regionObj.GetComponent<RegionHandler>().canMoveArmy = true;
                        }
                        //se a regiao for neutral
                        else if (regionObj.GetComponent<RegionHandler>().region.owner == "Neutral")
                        {
                            regionObj.GetComponent<RegionHandler>().canCreateArmy = false;
                            regionObj.GetComponent<RegionHandler>().canMoveArmy = true;
                        }
                        else //se a regiao for enimiga
                        {
                            regionObj.GetComponent<RegionHandler>().canCreateArmy = false;
                            regionObj.GetComponent<RegionHandler>().canMoveArmy = true;
                        }
                    }
                }
            }
            regionObj.GetComponent<RegionHandler>().isMoving = false;
            regionObj.GetComponent<RegionHandler>().isCreating = false;
            regionObj.GetComponent<RegionHandler>().createdThisRound = false;
        }
    }

    private void checkWin()
    {
        int defeatedCount = 0;
        foreach(Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            if (fac.defeated)
            {
                defeatedCount += 1;
            }
        }
        if(defeatedCount >= 3)
        {
            KeepWatchingButton.SetActive(false);
            InfoManager.Instance.hasGameEnded = true;
            roundUpdatesPanel.SetActive(false);
            optionsPanel.SetActive(false);
            winScreen.transform.Find("ResultPanel").gameObject.SetActive(true);
            if (Utils.GetMyFaction().defeated)
            {
                winScreen.transform.Find("ResultPanel").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("EndGameDefeat");
            }
            else
            {
                winScreen.transform.Find("ResultPanel").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("EndGameVictory");
            }
            winScreen.SetActive(true);
        }
    }

    private void CheckLose()
    {
        if (!InfoManager.Instance.hasGameEnded)
        {
            if (Utils.GetMyFaction().defeated)
            {
                winScreen.transform.Find("ResultPanel").gameObject.SetActive(true);
                winScreen.transform.Find("ResultPanel").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("EndGameDefeat");
                KeepWatchingButton.SetActive(true);
                winScreen.SetActive(true);
            }
        }
    }

    public void KeepWatching()
    {
        KeepWatchingButton.SetActive(false);
        winScreen.SetActive(false);
        winScreen.transform.Find("ResultPanel").gameObject.SetActive(false);
    }

    public void giveUp()
    {
        if (!Utils.GetMyFaction().defeated && !InfoManager.Instance.updating)
        {
            ServerResponse response = new ServerResponse();
            response.EventType = "Capitulation";
            response.data.Add(InfoManager.Instance.factionsManager.myFaction);
            GameCommunication.Instance.connection.SendMessage(JsonConvert.SerializeObject(response));
        }
        DisconnectFromServer();
        Application.Quit();
    }

    public void CloseGame()
    {
        DisconnectFromServer();
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        optionsButton.SetActive(false);
    }

    private void RoundUpdatesText()
    {
        if(InfoManager.Instance.factionsManager.updates.Count > 0)
        {
            roundUpdatesText.text = "												UPDATES" + "\n";
            foreach (string update in InfoManager.Instance.factionsManager.updates)
            {
                roundUpdatesText.text += update + "\n";
            }
        }
    }

    private void WinConditionText()
    {
        string designation;
        string description;
        Utils.GetMyFaction().defeatConditionValue.TryGetValue("designation", out designation);
        Utils.GetMyFaction().defeatConditionValue.TryGetValue("description", out description);
        winConditionText.text = "										WIN CONDITION" + "\n" +
            "Designation: " + designation + "\n" +
            "Descrition: " + description + "\n";
    }

    private void ShowFaction(JObject jsonResponse)
    {
        Text showFaction = loadingScreen.transform.Find("Faction").gameObject.GetComponent<Text>();

        foreach (object factionObj in jsonResponse["data"])
        {
            if (factionObj.ToString() != "")
            {
                try
                {
                    JObject factionJson = JObject.Parse(factionObj.ToString());
                    if (factionJson["playerId"].ToString() == this.userID)
                    {
                        showFaction.text = "Playing as " + factionJson["faction"].ToString();
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    private async Task GetArgumentsValues()
    {
        string protocolo = "";
        var args = Environment.GetCommandLineArgs();
        foreach (string arg in args)
        {
            if (arg.Contains("darkeningagegame"))
            {
                protocolo = arg;
            }
        }
        if (protocolo != "")
        {
            try
            {
                string[] delimiterStrings = { "darkeningagegame:", "room-id=", "&player-id=", "&player-auth=" };
                string[] arguments = protocolo.Split(delimiterStrings, StringSplitOptions.RemoveEmptyEntries);
                roomID = arguments[0];
                userID = arguments[1];
                userToken = arguments[2];
            }
            catch (Exception)
            {
                loadingScreen.transform.Find("Text").gameObject.SetActive(false);
                Text showFaction = loadingScreen.transform.Find("Faction").gameObject.GetComponent<Text>();
                showFaction.text = "START GAME FROM BROWSER";
                await Task.Delay(10000);
                Application.Quit();
            }
        }
        else
        {
            Task task = CloseGame("START GAME FROM BROWSER");
        }
    }

    private async Task CloseGame(string message)
    {
        loadingScreen.transform.Find("Text").gameObject.SetActive(false);
        Text showFaction = loadingScreen.transform.Find("Faction").gameObject.GetComponent<Text>();
        showFaction.text = message;
        await Task.Delay(10000);
        Application.Quit();
    }

    private Color SetScrollFactionColor(string factionName)
    {
        Color cor = new Color(1f, 1f, 1f, 1f);

        if(factionName == "Royalists")
        {
            cor = new Color(0f, 0.3844774f, 1f, 1f);
        }
        else if(factionName == "Confederation")
        {
            cor = new Color(1f, 0.979f, 0f, 1f);
        }
        else if (factionName == "Horde")
        {
            cor = new Color( 0f, 0.5943396f, 0.5943396f,1f);
        }
        else if (factionName == "Remnant")
        {
            cor = new Color(0.8207547f, 0.1152746f, 0f, 1f);
        }

        return cor;
    }
}

