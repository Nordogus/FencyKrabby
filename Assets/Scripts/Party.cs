using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//camille

public class Party : MonoBehaviour
{
    public static Party instance;

    // Entity
    public List<GameObject> entity = new List<GameObject>();

    // Score
    public int[] score = new int[4];
    public Text scoreTxt;

    // Timer
    public float timer = 180.0f;
    public Text timeText;
    public int timeArrond;
    public Text fightText;

    private bool _stopGame = false;
    public GameObject scenePrefab;
    private GameObject _topPlayer;

    public Text score0 = null;
    public Text score1 = null;
    public Text score2 = null;
    public Text score3 = null;

    public GameObject canvasResult = null;
    public GameObject[] crabs = new GameObject[4];

    [SerializeField] private GameObject[] _posEnd = new GameObject[4];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvasResult.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        for (int i = score.Length; i-- > 0;)
        {
            score[i] = 0;
        }

        StartCoroutine(Wait());
    }

    public IEnumerator Wait()
    {
        fightText.gameObject.SetActive(true);

        Debug.Log("0");
        fightText.text = "3";
        yield return new WaitForSeconds(1f);
        fightText.text = "2";
        yield return new WaitForSeconds(1f);
        fightText.text = "1"; 
        //yield return new WaitForSeconds(1f);
        //fightText.text = "0";
        yield return new WaitForSeconds(1f);
        fightText.text = "Fight";
        yield return new WaitForSeconds(1f);
        fightText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stopGame)
        {
            SetTime();
        }
    }

    public void AddScore(int id)
    {
        score[id]++;

        int idBestScore = -1;
        int besstScore = -1;
        for (int i = 4; i--> 0;)
        {
            if (besstScore < score[i])
            {
                besstScore = score[i];
                idBestScore = i;
            }
            else if (besstScore == score[i])
            {
                idBestScore = -1;
            }
        }

        if (idBestScore < 0)
        {
            _topPlayer = null;
        }
        else
        {
            _topPlayer = PlayerEntity.crabs[idBestScore].gameObject;
        }

        if (Crown.instance != null)
        {
            Crown.instance.SetObjectToFolo(_topPlayer);
        }
        // 0
        if (score[0] >= 10)
        {
            score0.text = score[0].ToString();
        }
        else
        {
            score0.text = "0" + score[0].ToString();
        }
        // 1
        if (score[1] >= 10)
        {
            score1.text = score[1].ToString();
        }
        else
        {
            score1.text = "0" + score[1].ToString();
        }
        // 2
        if (score[2] >= 10)
        {
            score2.text = score[2].ToString();
        }
        else
        {
            score2.text = "0" + score[2].ToString();
        }
        // 3
        if (score[3] >= 10)
        {
            score3.text = score[3].ToString();
        }
        else
        {
            score3.text = "0" + score[3].ToString();
        }
    }

    public void SetTime()
    {
        timer -= Time.deltaTime;
        timeArrond = Mathf.RoundToInt(timer);

        int tmpSecond = timeArrond % 60; 
        int tmpMin = (timeArrond - tmpSecond) / 60;

        if (tmpMin < 10)
        {
            if (tmpSecond < 10)
            {
                timeText.text = "0" + tmpMin.ToString() + " : " + "0" + tmpSecond.ToString();
                EndTime();
            }
            else
            {
                timeText.text = "0" + tmpMin.ToString() + " : " + tmpSecond.ToString();
            }
        }
        else
        {
            if (tmpSecond < 10)
            {
                timeText.text = tmpMin.ToString() + " : " + "0" + tmpSecond.ToString();
                EndTime();
            }
            else
            {
                timeText.text = tmpMin.ToString() + " : " + tmpSecond.ToString();
            }
        }
    }

    public void EndTime()
    {
        if (timer <= 0)
        {
            Debug.Log("Fin timer");
            System.Threading.Thread.Sleep(2000);

            timeText.gameObject.SetActive(false);

            timer = 0;
            Debug.Log("Fin Game");

            foreach (GameObject item in entity)
            {
                //Destroy(item);
            }
            _stopGame = true;
            canvasResult.SetActive(true);

            crabs[0].transform.position = _posEnd[0].transform.position;
            crabs[1].transform.position = _posEnd[1].transform.position;
            crabs[2].transform.position = _posEnd[2].transform.position;
            crabs[3].transform.position = _posEnd[3].transform.position;

            //System.Threading.Thread.Sleep(1000);
            StartCoroutine(Restart());
        }
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3f);
        scoreTxt.gameObject.SetActive(false);
        gameObject.transform.parent = null;
        Destroy(scenePrefab);
        SceneManager.LoadScene(0);
    }
}
