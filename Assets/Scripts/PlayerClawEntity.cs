using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Elise & Camille

public enum HitTipe
{
    noHit,
    hitCrab,
    hitShild,
}

public class PlayerClawEntity : MonoBehaviour
{
    #region Variables

    public List<Transform> initPosBase = new List<Transform>();
    public PlayerEntity myPlayer;
    [SerializeField] private GameObject _boddy = null;

    // MoveClaw
    private float _moveDirY;
    private Vector3 _initPosToMove = Vector3.zero;
    private Vector3 _backPos = Vector3.zero;

    public AnimationCurve curveNoneMoveAttaqueBack;
    public AnimationCurve curveShieldMoveAttaqueBack;
    public AnimationCurve curveCrabMoveAttaqueBack;


    // Attaque
    public bool canMakeDamage = true;
    public List<Transform> initPosAttaque = new List<Transform>();
    private bool _isAttaking = false;
    private bool _isAttakingBack = false;
    private float _attaqueDuration = 0f;
    private float _attaqueBackDuration = 0f;
    [SerializeField] private float _attaqueSpeed = 1f;
    [SerializeField] private float _noneAttaqueBackSpeed = 1f;
    [SerializeField] private float _shieldAttaqueBackSpeed = 1f;
    [SerializeField] private float _crabAttaqueBackSpeed = 1f;

    // retour d'attaque
    private bool _backAtk = true;
    private float ratio = 0f;
    [HideInInspector] public HitTipe hitTipe = HitTipe.noHit;

    // Orient
    private int pinceRotate = 0;

    // sound
    private bool _isAttaqueSound = false;

    // Debug
    public bool debugMode;

    // Particule
    [SerializeField] private GameObject _trailAttaque = null;       //ok


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponentInParent<PlayerEntity>();
        Rotat(myPlayer.clawDirection);

        if (_trailAttaque != null)
        {
            _trailAttaque.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (_isAttakingBack)
        {
            _UpdateAttaqueBack();
        }
        else if (_isAttaking)
        {
            _UpdateAttaque();
        }

        _UpdatePosition();
    }

    public void Move(float _dirY)
    {
        _moveDirY = _dirY;
    }

    public void _UpdatePosition()
    {
        float _tmpY = _moveDirY;
        Vector3 tmpPos = transform.position;

        tmpPos = Vector3.Lerp(initPosBase[pinceRotate].position, initPosAttaque[pinceRotate].position, ratio);

        if (!_isAttaking && !_isAttakingBack)
        {
            tmpPos = Vector3.Lerp(initPosBase[pinceRotate].position, initPosAttaque[pinceRotate].position, 0);
        }

        transform.position = tmpPos;
    }

    public void Attaque()
    {
        if (myPlayer.GetIsParry()) return;

        if (_isAttakingBack || _isAttaking)
        {
            return;
        }
        hitTipe = HitTipe.noHit;

        _isAttaking = true;
        _initPosToMove = transform.position;
        _attaqueDuration = 0f;

        if (_trailAttaque != null)
        {
            _trailAttaque.SetActive(true);
        }
    }

    public void Rotat(PlayerClawDirection clawDirection)
    {
        switch (clawDirection)
        {
            case PlayerClawDirection.North:
                pinceRotate = 0;
                transform.rotation = initPosAttaque[0].rotation;
                break;
            case PlayerClawDirection.South:
                pinceRotate = 1;
                transform.rotation = initPosAttaque[1].rotation;
                break;
            case PlayerClawDirection.East:
                pinceRotate = 2;
                transform.rotation = initPosAttaque[2].rotation;
                break;
            case PlayerClawDirection.West:
                pinceRotate = 3;
                transform.rotation = initPosAttaque[3].rotation;
                break;
            default:
                break;
        }
    }

    public void Counter()
    {
        canMakeDamage = false;
        _isAttakingBack = true;
        _isAttaking = false;
    }

    private void _UpdateAttaque()
    {
        canMakeDamage = true;
        
        if (!_isAttaqueSound)
        {
            AudioManager.instance.Play("Son_Mouvement_Pince");
            _isAttaqueSound = true;
        }

        if (_attaqueDuration >= 1f)
        {
            _isAttaking = false;
            _attaqueDuration = 1f;
            _isAttakingBack = true;
            _isAttaqueSound = false;
            canMakeDamage = false;

            if (_trailAttaque != null)
            {
                _trailAttaque.SetActive(false);
            }

            return;
        }
        _attaqueDuration += _attaqueSpeed * Time.fixedDeltaTime;
        ratio = _attaqueDuration;
    }

    private void _UpdateAttaqueBack()
    {
        if (_isAttaking)
        {
            _isAttaking = false;
            _attaqueDuration = 1f;
            _isAttakingBack = true;
            _isAttaqueSound = false;
            canMakeDamage = false;

            if (_trailAttaque != null)
            {
                _trailAttaque.SetActive(false);
            }
        }

        if (_backAtk)
        {
            canMakeDamage = false;

            _backAtk = false;
            _attaqueBackDuration = ratio;
        }



        switch (hitTipe)
        {
            case HitTipe.noHit:     //attaque dans le vide
                _attaqueBackDuration -= _noneAttaqueBackSpeed * Time.fixedDeltaTime;
                ratio = curveNoneMoveAttaqueBack.Evaluate(_attaqueBackDuration);
                break;
            case HitTipe.hitCrab:   // attaque touche le crab
                _attaqueBackDuration -= _crabAttaqueBackSpeed * Time.fixedDeltaTime;
                ratio = curveCrabMoveAttaqueBack.Evaluate(_attaqueBackDuration);
                break;
            case HitTipe.hitShild:  // attaque touche un bouclier
                _attaqueBackDuration -= _shieldAttaqueBackSpeed * Time.fixedDeltaTime;
                ratio = curveShieldMoveAttaqueBack.Evaluate(_attaqueBackDuration);
                break;
            default:
                _attaqueBackDuration -= _attaqueSpeed * Time.fixedDeltaTime;
                ratio = _attaqueBackDuration;
                break;
        }

        if (_attaqueBackDuration <= 0f)
        {
            _isAttakingBack = false;
            _isAttaking = false;
            _attaqueBackDuration = 0f;
            _backAtk = true;
            onDomage = false;
            return;
        }
    }

    private bool onDomage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _boddy) return;
        if (other.gameObject.tag == "CrabBoddy")
        {
            if (canMakeDamage && !onDomage)
            {
                CameraShaker.Instance.ShakeOnce(0.5f, 1f, 0.1f, 1f);
                PlayerEntity _lPlayer = other.gameObject.GetComponentInParent<PlayerEntity>();
                if (_lPlayer == myPlayer) return;
                    _UpdateAttaqueBack();
                if (_lPlayer.TakeDamage(1, this))
                {
                    Party.instance.AddScore(myPlayer.idCrab);
                }
                onDomage = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "CrabBoddy")
        {
            onDomage = false;
        }
    }

    private void OnGUI()
    {
        if (!debugMode) { return; }

        GUILayout.BeginVertical();
        GUILayout.Label(" pinceRotate : " + pinceRotate);
        GUILayout.EndVertical();
    }
}
