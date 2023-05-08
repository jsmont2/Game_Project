using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class reset : MonoBehaviour
{
    public void RestartButton()
    {
        SceneManager.LoadScene("overworld");
    }
}
