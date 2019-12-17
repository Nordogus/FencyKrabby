using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camille

public class LifeBar : MonoBehaviour
{
    public float _life = 2f;
    public float _lifeMax = 2f;
    private Vector3 _initScale = Vector3.zero;

    private void Start()
    {
        _initScale = transform.localScale;
    }

    public void SetLife(float life, float lifeMax)
    {
        Vector3 tmpScale = transform.localScale;
        tmpScale.x = life/lifeMax * _initScale.x;
        transform.localScale = tmpScale;
    }
}
