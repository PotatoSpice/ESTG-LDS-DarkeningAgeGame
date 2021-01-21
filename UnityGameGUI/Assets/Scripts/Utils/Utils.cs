using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utils
{
    static class Utils
    {
        public static bool CheckMouseOverUI()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Faction GetMyFaction()
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

        public static GameObject GetRegion(string region)
        {
            MapManager mapManager = new MapManager();
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
}
