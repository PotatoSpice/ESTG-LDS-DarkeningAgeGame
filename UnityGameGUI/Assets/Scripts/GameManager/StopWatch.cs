using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StopWatch : MonoBehaviour
{
    public Text time;
    public Text round;
    public int roundNum;
    private bool takingAway;
    private int timeNum;
    private bool roundStart;
    private int actionsTime;
    public GameObject updateText;
    public GameObject updatePanel;

    public GameObject regionOptionsPanel;
    public GameObject regionWarMenuPanel;
    public GameObject regionInfo;

    void Start()
    {
        takingAway = false;
        roundStart = false;
    }

    private void Update()
    {
        if (roundStart)
        {
            StartCount();
        }
    }
    
    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        timeNum -= 1;
        if(timeNum > actionsTime)
        {
            if (!updateText.activeSelf)
            {
                updateText.SetActive(true);
            }
            else
            {
                updateText.SetActive(false);
            }
            
            time.text = "<b>  Time: </b>00:" + (timeNum - actionsTime);
        }
        if(timeNum == actionsTime)
        {
            regionOptionsPanel.SetActive(false);
            regionWarMenuPanel.SetActive(false);
            regionInfo.SetActive(false);
            updateText.SetActive(false);
            updatePanel.SetActive(false);
            time.text = "<b>  Time: </b>00:" + timeNum;
            InfoManager.Instance.updating = false;
        }
        if(timeNum < actionsTime)
        {
            time.text = "<b>  Time: </b>00:" + timeNum;
        }
        takingAway = false;
    }

    private void StartCount()
    {
        if (takingAway == false && timeNum > 0)
        {
            StartCoroutine(TimerTake());
        }
    }

    public void SetRound(JObject jsonResponse)
    {
        roundStart = true;
        InfoManager.Instance.updating = true;

        timeNum = 60;
        actionsTime = 45;

        roundNum = Convert.ToInt32(jsonResponse["turn"].ToString());
        round.text = "<b>  Round: </b>" + roundNum;
        time.text = "<b>  Time: </b>00:" + (timeNum-actionsTime);
    }
}
