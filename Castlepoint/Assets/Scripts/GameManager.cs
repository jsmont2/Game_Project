using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager instance;

    // Player hearts
    private int playerHearts = 3;

    // Public property to access hearts
    public int PlayerHearts
    {
        get { return playerHearts; }
        set { playerHearts = value; }
    }

    private void Awake()
    {
        // Ensure there is only one instance of GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
