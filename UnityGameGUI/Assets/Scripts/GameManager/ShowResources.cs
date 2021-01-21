using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ShowResources : MonoBehaviour
{
    public Text gold;
    public Text food;
    public Text wood;
    public Text manPower;
    public Text goldMaintenance;
    public Text foodMaintenance;
    public Text woodMaintenance;
    public Text time;
    public Text round;

    public GameObject resources;

    private void Update()
    {
        if(Utils.GetMyFaction() != null)
        {
            UpdateResources();

            if (Utils.GetMyFaction().defeated || InfoManager.Instance.hasGameEnded)
            {
                resources.SetActive(false);
                if (InfoManager.Instance.hasGameEnded)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateResources()
    {
        Faction faction = Utils.GetMyFaction();

        gold.text = faction.gold.ToString();
        food.text = faction.food.ToString();
        wood.text = faction.wood.ToString();
        manPower.text = faction.manpower.ToString();
        foodMaintenance.text = faction.foodMaintenance.ToString();
        goldMaintenance.text = faction.goldMaintenance.ToString();
        woodMaintenance.text = faction.woodMaintenance.ToString();
    }
}
