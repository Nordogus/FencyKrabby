using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Settings
{
    public string language = "en";
    public int volume = 100;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Settings option = new Settings();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // loading
        LoadSave();

        MenuManager.instance.StartManager();
    }

    private void LoadSave()
    {
        if (SaveLoad.SaveExists("Option"))
        {
            option = SaveLoad.Load<Settings>("Option");
        }
        else
        {
            SaveLoad.Save<Settings>(option, "Option");
        }
    }

    public void SaveSettings()
    {
        SaveLoad.Save<Settings>(option, "Option");
    }

    public void ResteSettings()
    {
        option = new Settings();
        SaveLoad.Save<Settings>(option, "Option");
    }
}
