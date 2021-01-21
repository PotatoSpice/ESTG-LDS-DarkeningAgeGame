using Assets.Scripts.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager
{
    private GameObject[] regions;

    public MapManager()
    {
        GetRegionsObjects();
    }

    public void GetRegionsObjects()
    {
        this.regions = GameObject.FindGameObjectsWithTag("Region");
    }

    public void SetRegionsValues(JObject jsonResponse)
    {
        List<GameObject> regionNotUpdated = regions.ToList();
        List<RegionType> regionsType = new List<RegionType>();

        //Regions Type Info
        foreach (object regionObj in jsonResponse["regionTypes"])
        {
            if (regionObj != null)
            {
                try
                {
                    regionsType.Add(JsonConvert.DeserializeObject<RegionType>(regionObj.ToString()));
                }
                catch (Exception e)
                {
                }
            }
        }

        //Regions Info
        foreach (object regionObj in jsonResponse["regions"])
        {
            if (regionObj != null)
            {
                try
                {
                    JObject regionJson = JObject.Parse(regionObj.ToString());
                    foreach (GameObject region in regionNotUpdated)
                    {
                        RegionHandler regionComponent = region.GetComponent<RegionHandler>();
                        if (regionComponent.region.name == regionJson["name"].ToString())
                        {
                            regionComponent.region.owner = regionJson["owner"].ToString();

                            SetRegionsColors(regionComponent);

                            regionComponent.region.terrain = regionJson["terrain"].ToString();
                            regionComponent.region.size = float.Parse(regionJson["size"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                            foreach (string border in regionJson["Borders"])
                            {
                                regionComponent.region.borders.Add(border);
                            }

                            foreach (RegionType regionType in regionsType)
                            {
                                if (regionType.type == regionJson["type"].ToString())
                                {
                                    regionComponent.region.type = regionType;
                                }
                            }
                            regionNotUpdated.Remove(region);
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    public void UpdateRegionsOwners(JObject jsonResponse)
    {
        List<GameObject> regionNotUpdated = regions.ToList();

        foreach (GameObject region in regionNotUpdated)
        {
            region.GetComponent<RegionHandler>().armyImage.SetActive(false);
        }

        foreach (object regionObj in jsonResponse["regions"])
        {
            if (regionObj != null)
            {
                try
                {
                    JObject regionJson = JObject.Parse(regionObj.ToString());
                    foreach (GameObject region in regionNotUpdated)
                    {
                        RegionHandler regionComponent = region.GetComponent<RegionHandler>();
                        if (regionComponent.region.name == regionJson["name"].ToString())
                        {
                            if (regionComponent.region.owner == InfoManager.Instance.factionsManager.myFaction)
                            {
                                if (regionComponent.region.owner != regionJson["owner"].ToString())
                                {
                                    InfoManager.Instance.factionsManager.updates.Add("You lost the region " + regionComponent.region.name);
                                }
                            }
                            else
                            {
                                if (InfoManager.Instance.factionsManager.myFaction == regionJson["owner"].ToString())
                                {
                                    InfoManager.Instance.factionsManager.updates.Add("You conquered region " + regionComponent.region.name);
                                }
                            }

                            regionComponent.region.owner = regionJson["owner"].ToString();

                            SetRegionsColors(regionComponent);
                            if (!Utils.GetMyFaction().defeated)
                            {
                                SetRegionArmyImage(regionComponent, false);
                            }
                            else
                            {
                                ShowAllArmies(regionComponent);
                            }

                            regionNotUpdated.Remove(region);
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    public List<GameObject> GetRegions()
    {
        return regions.ToList();
    }

    private void SetRegionsColors(RegionHandler regionComponent)
    {
        if (regionComponent.region.owner == "Neutral")
        {
            regionComponent.defaultColor = new Color(0f, 0f, 0f, 0f);
        }
        if (regionComponent.region.owner == "Horde")
        {
            regionComponent.defaultColor = new Color(0.008675906f, 0.4716981f, 0f, 0.7803922f);
            /*regionComponent.armyImage.GetComponent<SpriteRenderer>().color = new Color(0.02936988f, 0.4150943f, 0.04760263f, 1f);*/
        }
        if (regionComponent.region.owner == "Remnant")
        {
            regionComponent.defaultColor = new Color(0.702f, 0.05896226f, 0.09344167f, 0.7803922f);
            /*regionComponent.armyImage.GetComponent<SpriteRenderer>().color = new Color(0.4245283f, 0.03003738f, 0.05168634f, 1f);*/
        }
        if (regionComponent.region.owner == "Royalists")
        {
            regionComponent.defaultColor = new Color(0f, 0.04374336f, 0.6415094f, 0.7803922f);
            /*regionComponent.armyImage.GetComponent<SpriteRenderer>().color = new Color(0.0259434f, 0.03974746f, 0.5f, 1f);*/
        }
        if (regionComponent.region.owner == "Confederation")
        {
            regionComponent.defaultColor = new Color(1f, 0.7514094f, 0.0518868f, 0.7803922f);
            /*regionComponent.armyImage.GetComponent<SpriteRenderer>().color = new Color(0.6037736f, 0.5608589f, 0f, 1f);*/
        }
        regionComponent.armyImage.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
        regionComponent.sprite.color = regionComponent.defaultColor;
    }

    private void SetRegionArmyImage(RegionHandler region, bool isBorder)
    {
        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            if (fac.name == InfoManager.Instance.factionsManager.myFaction || isBorder)
            {
                foreach (Army ar in fac.armies)
                {
                    if (ar.region == region.region.name)
                    {
                        region.armyImage.SetActive(true);

                        if (!isBorder)
                        {
                            foreach (GameObject border in region.borders)
                            {
                                if (border.GetComponent<RegionHandler>().region.owner != InfoManager.Instance.factionsManager.myFaction)
                                {
                                    SetRegionArmyImage(border.GetComponent<RegionHandler>(), true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ShowAllArmies(RegionHandler region)
    {
        foreach (Faction fac in InfoManager.Instance.factionsManager.factions)
        {
            foreach (Army ar in fac.armies)
            {
                if (ar.region == region.region.name)
                {
                    region.armyImage.SetActive(true);
                }
            }
        }
    }
}
