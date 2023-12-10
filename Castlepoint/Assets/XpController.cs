using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class XpController : MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {
        currentXp = data.currentXp;
    }
    public void SaveData(GameData data)
    {
        data.currentXp = currentXp;
    }
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ExpText;
    public int level;
    public float currentXp;
    public float TargetXp;
    public Image XpProgressBar;

    // For level up text
    public GameObject levelUpText;

    // For level up sound
    public AudioClip levelUpSound;
    private AudioSource levelUpAudio;

    private void Start()
    {
        // Initialize the AudioSource
        levelUpAudio = gameObject.AddComponent<AudioSource>();
        levelUpAudio.clip = levelUpSound;
        levelUpAudio.playOnAwake = false;
    }

    private void Update()
    {

        ExpText.text = currentXp + " / " + TargetXp;

        ExperienceController();
        
    }

    // This one doesn't seem to work for UI components...
    public void ExperienceController()
    {
        LevelText.text = "Level: " + level.ToString();
        XpProgressBar.fillAmount = (currentXp / TargetXp);

        if (currentXp >= TargetXp)
        {
            
            currentXp = currentXp - TargetXp;
            level++;
            TargetXp += 50;

         
        }
    }

    // This method works for UI components
    public void AddExperience(int xpAmount)
    {
        currentXp += xpAmount;
        ExpText.text = currentXp + " / " + TargetXp;

        // Handle leveling up logic
        if (currentXp >= TargetXp)
        {
            currentXp = currentXp - TargetXp;
            level++;
            TargetXp += 50;

            if (levelUpText != null)
            {

                levelUpText.SetActive(true);
                StartCoroutine(FlickerLevelUpText());
                if (levelUpAudio != null && levelUpSound != null)
                {
                    levelUpAudio.Play();
                }
            }
        }
    }

    public void PinkSlimeDestroyed()
    {
        AddExperience(20); // or any other XP amount you want to grant
    }

    IEnumerator FlickerLevelUpText()
    {
        float duration = 2f; // Adjust the total duration as needed
        float flickerInterval = 0.1f; // Adjust the interval between flickers

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            levelUpText.SetActive(!levelUpText.activeSelf); // Toggle visibility

            yield return new WaitForSeconds(flickerInterval);

            elapsedTime += flickerInterval;
        }

        levelUpText.SetActive(false); // Ensure the text is hidden after the flickering
    }

}
