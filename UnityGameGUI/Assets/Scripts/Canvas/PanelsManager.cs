using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class PanelsManager : MonoBehaviour
{
    //Chat Variables
    public GameObject chatPanel;
    public ChatManager chat;
    public Text chatText;
    public GameObject chatSendButton;
    public GameObject inputFieldObject;
    public InputField inputText;

    public GameObject regionPanel;
    public GameObject regionActions;
    public GameObject warOptions;

    public GameObject createArmyInvisible;
    public GameObject createArmyPanel;
    public GameObject addCharacter;

    public GameObject checkArmyPanel;
    public GameObject addUnitToArmy;
    public GameObject checkUnitPanel;

    // Start is called before the first frame update
    void Start()
    {
        chat = new ChatManager(chatText, inputText);

        chatPanel.SetActive(false);

        CloseAllPanels();
    }

    // Update is called once per frame
    void Update()
    {
        ChatState();
        if (InfoManager.Instance.hasGameEnded)
        {
            chatPanel.SetActive(false);
        }
    }

    public void RegionPanelClose()
    {
        regionPanel.SetActive(false);
        regionActions.SetActive(false);
    }

    public void CreateArmyPanelClose()
    {
        createArmyInvisible.SetActive(false);
        createArmyPanel.SetActive(false);
    }

    public void CheckArmyPanelClose()
    {
        createArmyInvisible.SetActive(false);
        checkArmyPanel.SetActive(false);
    }

    public void CheckUnitPanelClose()
    {
        checkUnitPanel.SetActive(false);
    }

    //Change chat state
    private void ChatState()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (chatPanel.activeSelf == true && chat.GetInputText() == "")
            {
                chatPanel.SetActive(false);
            }
            else
            {
                chatPanel.SetActive(true);
                if (GetMyFaction().defeated)
                {
                    chatSendButton.SetActive(false);
                    inputFieldObject.SetActive(false);
                }
                
                EventSystem.current.SetSelectedGameObject(inputText.gameObject, null);
            }
        }
    }

    public void CloseAllPanels()
    {
        regionPanel.SetActive(false);
        regionActions.SetActive(false);
        warOptions.SetActive(false);

        createArmyInvisible.SetActive(false);
        createArmyPanel.SetActive(false);
        addCharacter.SetActive(false);

        checkArmyPanel.SetActive(false);
        addUnitToArmy.SetActive(false);
        checkUnitPanel.SetActive(false);
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
}