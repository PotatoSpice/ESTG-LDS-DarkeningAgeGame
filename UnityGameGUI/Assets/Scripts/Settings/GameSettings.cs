using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{

    public GameObject resolutionsPanel;
    public Dropdown resolutionsDropdown;
    public Toggle toggle;

    private Resolution[] resolutions;

    private void Awake()
    {
        int startWidth = PlayerPrefs.GetInt("ResolutionWidth", -1);
        int startHeight = PlayerPrefs.GetInt("ResolutionHeight", -1);
        string startFullScreen = PlayerPrefs.GetString("ResolutionFullScreen", "");
        if (startWidth != -1 && startHeight != -1 && startFullScreen != "")
        {
            Screen.SetResolution(startWidth, startHeight, Boolean.Parse(startFullScreen));
            if (Boolean.Parse(startFullScreen))
            {
                resolutionsPanel.SetActive(false);
            }
            else
            {
                resolutionsPanel.SetActive(true);
                toggle.isOn = false;
            }
        }
        else
        {
            resolutionsPanel.SetActive(false);
        }

        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        List<Resolution> newResolutions = new List<Resolution>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            float constant = resolutions[i].width / 16;
            float result = resolutions[i].height / constant;
            if (result == 9.0f)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                if (!options.Contains(option))
                {
                    options.Add(option);
                    newResolutions.Add(resolutions[i]);
                }

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
        }
        resolutions = newResolutions.ToArray();

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    void Start()
    {

    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        
        resolutionsPanel.SetActive(!isFullScreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        PlayerPrefs.SetString("ResolutionFullScreen", Screen.fullScreen.ToString());
    }
}
