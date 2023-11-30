using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;



public class OptionsMenu1 : MonoBehaviour
{

    public AudioMixer audioMixer;

    public Dropdown resolutionDropdown;

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    Resolution[] resolutions;
    /* the code first clears our options in the resolution dropdown. then it creates a list of strings which are our options.
     * we loop through each element on the resolutions array that will create a string taht will display our resolutions a format width x height
     * and then we add it to the options list we lastly add our options list into the resolutionDropdown.
     */
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currenResolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currenResolution = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currenResolution;
        resolutionDropdown.RefreshShownValue();

    }
    //The code bellow is the Audio mixer for the options menu and controls the volume of our game. I added a debug function to show tha it is working.

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("Volume", volume);
    }


    //The code below is used to control the graphics dropdown and if you want to add more option on unity go to edit, project settings and look for the qulity tab there you can add more option and edit how each category rebders.
    public void SetQuality(int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);
    }
    // the code bellow is for the fullscreen toggle in options menu and can only be seen once we build the game.
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }


}
