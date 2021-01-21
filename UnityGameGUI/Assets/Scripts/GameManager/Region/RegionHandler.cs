using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]

public class RegionHandler : MonoBehaviour
{
    public SpriteRenderer sprite;

    public Color defaultColor;
    private Color hoverColor;

    public Text regionInfo;
    public GameObject regionPanel;
    public GameObject regionActionOptions;

    public GameObject warOptionsPanel;

    ////Actions Available
    //if region can create army
    public bool canCreateArmy;
    //if region can receive army
    public bool canMoveArmy;
    //if region has army moving
    public bool isMoving;
    //if region has units creating
    public bool isCreating;
    //if region created army this round
    public bool createdThisRound;

    //Region Info
    public Region region;
    public string regionName;

    public GameObject armyPrefab;
    public GameObject armyImage;

    public List<GameObject> borders;

    private void Start()
    {
        canCreateArmy = true;
        canMoveArmy = false;
        isMoving = false;
        isCreating = false;
        region = new Region(regionName);

        hoverColor = new Color(0f, 0f, 0f, 0.6666667f);
        defaultColor = new Color(0f, 0f, 0f, 0f);

        sprite.color = defaultColor;

        this.SetArmy();
    }

    private void Update()
    {
        if (borders.Count == 0)
        {
            borders = GetBorders();
            sprite.color = defaultColor;
        }
        if (InfoManager.Instance.hasGameEnded)
        {
            regionActionOptions.SetActive(false);
            warOptionsPanel.SetActive(false);
            regionPanel.SetActive(false);
        }
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

    }

    private void OnMouseDown()
    {
        if (!InfoManager.Instance.hasGameEnded)
        {
            if (InfoManager.Instance.canMoveArmy == true)
            {
                ShowRegionDetails();
            }
            else
            {
                InfoManager.Instance.destinyRegion = region.name;
            }
        }
        //HighLightBorders();
    }

    private void OnMouseUp()
    {
        //DefaultBorderColor();
    }

    void OnMouseEnter()
    {
        sprite.color = hoverColor;
    }

    void OnMouseExit()
    {
        sprite.color = defaultColor;
    }

    private void ShowRegionDetails()
    {
        if (Utils.CheckMouseOverUI())
        {
            InfoManager.Instance.selectedRegion = region.name;
            if (!Utils.GetMyFaction().defeated)
            {
                regionActionOptions.SetActive(true);
            }
            warOptionsPanel.SetActive(false);
            regionPanel.SetActive(true);

            regionInfo.text = "\n" + "    <b>Name: </b>" + region.name + "\n" +
            "    <b>Owner: </b>" + region.owner + "\n" +
            "    <b>Terrain: </b>" + region.terrain + "\n" +
            "    <b>Type: </b>" + region.type.type + "\n" +
            "    <b>Man Power: </b>" + region.type.manpower + "\n" +
            "    <b>Gold: </b>" + region.type.gold + "\n" +
            "    <b>Wood: </b>" + region.type.wood + "\n" +
            "    <b>Food: </b>" + region.type.food + "\n" +
            "    <b>Defence Bonus: </b>" + region.type.defencebonus + "\n";
        }
    }

    public void HighLightBorders()
    {
        foreach (GameObject border in borders)
        {
            border.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0.7279429f, 0.6156863f);
        }
    }

    public void DefaultBorderColor()
    {
        foreach (GameObject border in borders)
        {
            border.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public List<GameObject> GetBorders()
    {
        List<GameObject> regionB = new List<GameObject>();
        GameObject[] regionsArray = GameObject.FindGameObjectsWithTag("Region");

        foreach (string border in region.borders)
        {
            foreach (GameObject regionIndex in regionsArray)
            {
                if (border == regionIndex.GetComponent<RegionHandler>().region.name)
                {
                    regionB.Add(regionIndex);
                }
            }
        }
        return regionB;
    }

    private void SetArmy()
    {
        Vector3 center = transform.position;
        this.armyImage = (Instantiate(armyPrefab, transform));
        this.armyImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        center.y = center.y + (this.armyImage.GetComponent<SpriteRenderer>().size.y / 20);
        this.armyImage.transform.position = center;
        this.armyImage.GetComponent<SpriteRenderer>().sortingOrder = -1;
        
        this.armyImage.SetActive(false);
    }
}