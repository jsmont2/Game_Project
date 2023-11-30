using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    /*  
        The code below is for the Play Button and it pretty much loads the next scene.
        If you want to change the scene after clicking play go to file on Unity and under the build settings tab you will see the order in which scenes play.
    */
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
    }

    // The code below quits the program when you press quit in the MainMenu.
    // Debug allows us to see that the Quit button is functioning it obly will work when we build our game and wont work in unity editor.
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }


}