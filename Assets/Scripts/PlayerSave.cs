using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    
    public static PlayerSave Instance;

    public int day = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // do other
            Destroy(gameObject);
            return;
        }

        // save between scenes
        DontDestroyOnLoad(gameObject);

        // get save
        if (PlayerPrefs.HasKey("Day"))
        {
            day = PlayerPrefs.GetInt("Day");
        } 
        else
        {
            day = 0;
        }
    }

}
