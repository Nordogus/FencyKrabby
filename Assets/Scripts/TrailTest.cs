using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTest : MonoBehaviour
{
    private TrailRenderer ClawTrail; 
  

    // Start is called before the first frame update
    void Start()
    {
         ClawTrail = gameObject.GetComponent<TrailRenderer>();
        ClawTrail.enabled = false;         
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            ClawTrail.enabled = true;

        }
       
        if (Input.GetKeyDown(KeyCode.A))
        {
            ClawTrail.enabled = false; 
        }

    }
}
