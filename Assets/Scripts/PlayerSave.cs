using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    
    public static PlayerSave Instance;

    public int day = 0;

    public float masterVolume = 0.5f;
    public float sfxVolume = 1.0f;
    public float ambianceVolume = 0.5f;

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
        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.SetFloat("Master-Volume", masterVolume);
        PlayerPrefs.SetFloat("SFX-Volume", sfxVolume);
        PlayerPrefs.SetFloat("Ambiance-Volume", ambianceVolume);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Master-Volume", masterVolume);
        PlayerPrefs.SetFloat("SFX-Volume", sfxVolume);
        PlayerPrefs.SetFloat("Ambiance-Volume", ambianceVolume);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Day"))
        {
            day = PlayerPrefs.GetInt("Day");
        }
        else
        {
            day = 0;
        }

        if (PlayerPrefs.HasKey("Master-Volume"))
        {
            masterVolume = PlayerPrefs.GetFloat("Master-Volume");
        } else
        {
            masterVolume = 0.5f;
        }

        if (PlayerPrefs.HasKey("SFX-Volume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFX-Volume");
        } else
        {
            sfxVolume = 1.0f;
        }

        if (PlayerPrefs.HasKey("Ambiance-Volume"))
        {
            ambianceVolume = PlayerPrefs.GetFloat("Ambiance-Volume");
        }
        else
        {
            ambianceVolume = 0.5f;
        }
    }

}
