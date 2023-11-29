using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class XpController : MonoBehaviour
{
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ExpText;
    public int level;
    public float currentXp;
    public float TargetXp;
    public Image XpProgressBar;

    private void Update()
    {

        ExpText.text = currentXp + " / " + TargetXp;

        ExperienceController();
        
    }

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
        }
    }

    public void PinkSlimeDestroyed()
    {
        AddExperience(20); // or any other XP amount you want to grant
    }

}
