using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject ecranPause;

    // Start is called before the first frame update
    void Start()
    {
        ecranPause.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ecranPause.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void BackToGame()
    {
        ecranPause.SetActive(false);

        Time.timeScale = 1;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;

        //SceneManager.LoadScene(0);
    }

    public void Debug()
    {
        
        Party.instance.timer = 0;
    }
}
