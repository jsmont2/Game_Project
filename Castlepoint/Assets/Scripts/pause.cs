using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject theData;

    public GameObject pauseSound; 
    public GameObject resumeSound;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Debug.Log("Resume button clicked!");
                Resume();
                PlayResumeSound();
            }
            else
            {
                Pause();
                PlayPauseSound();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayResumeSound();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        //PlayerPrefs.SetInt("SavedScene", SceneManager.GetActiveScene().buildIndex);
        theData = GameObject.Find("DataPersistenceManager"); 
        theData.GetComponent<DataPersistenceManager>().SaveGame();       
        SceneManager.LoadSceneAsync(0);
        
        Time.timeScale = 1f;
        
    }
    public void MuteToggle(bool muted)
    {
        if (muted)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    void PlayPauseSound()
    {
        if (pauseSound != null)
        {
            pauseSound.GetComponent<AudioSource>().Play();
        }
    }

    void PlayResumeSound()
    {
        if (resumeSound != null)
        {
            resumeSound.GetComponent<AudioSource>().Play();
        }
    }

}
