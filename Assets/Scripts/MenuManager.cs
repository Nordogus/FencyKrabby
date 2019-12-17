using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private GameObject[] _BackGround = new GameObject[2];
    [SerializeField] private GameObject _canvasPlayerSelection = null;
    [SerializeField]
    private GameObject canvasMenu = null;
    [SerializeField]
    private GameObject gameTitle = null;
    [SerializeField]
    private bool tittleScreen = false;
    [SerializeField]
    private List<GameObject> canvasList = new List<GameObject>();

    [SerializeField] private EventSystem _event = null;

    //fade variables
    [SerializeField]
    private RawImage cache = null;
    private Color tmpColor;
    [SerializeField]
    private float speedFade = 1;
    [SerializeField]
    private bool inFadeOut = false;
    [SerializeField]
    private bool inFadeIn = false;

    // Load Perso
    [Header("Perso")]
    [SerializeField] private List<GameObject> _playerIcone = new List<GameObject>();
    private bool _inPartyCreation = false;
    private bool[] _playerReaddy = new bool[4];
    private bool _allPlayerReaddy = false;

    //Rewired
    private List<Player> _rewiredPlayers = new List<Player>();  // liste des controleur

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

        foreach (GameObject item in canvasList)
        {
            item.SetActive(false);
        }
        cache.gameObject.SetActive(true);

        if (tittleScreen)
        {
            canvasMenu.SetActive(false);
            gameTitle.SetActive(false);
        }
        else
        {
            StartManager();
        }
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _rewiredPlayers.Add(ReInput.players.GetPlayer(i));
        }
    }

    public void SetFocus(GameObject focus)
    {
        _event.firstSelectedGameObject = focus;
    }

    public void StartManager()
    {
        tmpColor.a = 1;
        if (tittleScreen)
        {
            canvasMenu.SetActive(true);
            gameTitle.SetActive(true);
        }
        inFadeOut = true;
    }

    public void BackMenu()
    {
        _inPartyCreation = false;
        _BackGround[0].SetActive(true);
        _BackGround[1].SetActive(true);
        _canvasPlayerSelection.SetActive(false);
    }

    private void Update()
    {
        Faid();

        if (_inPartyCreation)
        {
            WaitForReaddy();
        }
    }

    public void SetBoolTrue(bool ok)
    {
        _inPartyCreation = ok;
        _BackGround[0].SetActive(false);
        _BackGround[1].SetActive(false);
        _canvasPlayerSelection.SetActive(true);
    }

    private void WaitForReaddy()
    {
        for (int i = 0; i < 4; i++)
        {

            if (_allPlayerReaddy)
            {
                if (_rewiredPlayers[i].GetButtonDown("Start"))
                {
                    AudioManager.instance.Play("Son_Choc_Crabe");

                    LoadScene(1);
                }
            }
            if (_rewiredPlayers[i].GetButtonDown("Activate"))
            {
                AudioManager.instance.Play("Son_Choc_Crabe");

                SetReaddy(_playerIcone[i], true);
                _playerReaddy[i] = true;
            }

            if (_rewiredPlayers[i].GetButtonDown("Cancel"))
            {
                AudioManager.instance.Play("Son_Choc_Crabe");

                SetReaddy(_playerIcone[i], false);
                _playerReaddy[i] = false;
            }

            if (_playerReaddy[0] && _playerReaddy[1] && _playerReaddy[2] && _playerReaddy[3])
            {
                _allPlayerReaddy = true;
            }
            else
            {
                _allPlayerReaddy = false;
            }

            if (_rewiredPlayers[i].GetButtonDown("Taunt") && _playerReaddy[i])
            {
                switch (i)
                {
                    case 0:
                        AudioManager.instance.Play("Rouge_punchline");
                        break;
                    case 1:
                        AudioManager.instance.Play("Bleu_punchline");
                        break;
                    case 2:
                        AudioManager.instance.Play("Vert_punchline");
                        break;
                    case 3:
                        AudioManager.instance.Play("Jaune_punchline");
                        break;
                }
            }
        }

    }

    private void SetReaddy(GameObject playerId, bool on)
    {
        if (on)
        {
            playerId.SetActive(true);
        }
        else
        {
            playerId.SetActive(false);
        }
    }

    public void ActivateCanvas(GameObject _canvas)
    {
        foreach (GameObject item in canvasList)
        {
            item.SetActive(false);
        }
        if (tittleScreen)
        {
            gameTitle.SetActive(false);
        }
        _canvas.SetActive(true);
    }

    public void UnactiveCanvas(GameObject _canvas)
    {
        _canvas.SetActive(false);
        if (tittleScreen)
        {
            gameTitle.SetActive(true);
        }
    }

    private void Faid()
    {
        if (inFadeIn)
        {
            FadeIn();
        }
        else if (inFadeOut)
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        tmpColor.a += speedFade * Time.deltaTime;
        cache.color = tmpColor;

        if (tmpColor.a >= 1)
        {
            inFadeIn = false;
        }
    }

    private void FadeOut()
    {
        tmpColor.a -= speedFade * Time.deltaTime;
        cache.color = tmpColor;
        if (tmpColor.a <= 0)
        {
            inFadeOut = false;
        }
    }
    
    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void LoadScene(int idScene)
    {
        SceneManager.LoadScene(idScene);
        SetBoolTrue(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
