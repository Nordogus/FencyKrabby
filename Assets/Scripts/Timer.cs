using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public float time = 180.0f;
    public Text timeText;
    public int timeArrond;

    // Update is called once per frame
    void Update()
    {
        BeginTime();
        SetTime();
        EndTime();
    }


    public void OnGUI()
    {
        GUI.Box(new Rect (10,10,50,25), time.ToString("0"));
    }


    public void BeginTime()
    {
    }

    public void SetTime()
    {
        time -= Time.deltaTime;
        timeArrond = Mathf.RoundToInt(time);
        //Debug.Log("time" + time);
    }

    public void EndTime()
    {
        if(time <= 0)
        {
            time = 0;
            Debug.Log("Fin timer");
        }

    }
}
