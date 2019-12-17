using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

// Camille

public enum PlayerClawDirection
{
    North,
    South,
    East,
    West
}

public class PlayerEntity : MonoBehaviour
{
    //public static List<PlayerEntity> crabs = new List<PlayerEntity>();
    public static PlayerEntity[] crabs = new PlayerEntity[4];

    #region Varialbes

    //Crab
    [Header("Crab")]
    public bool _isAlive;
    public GameObject allInCrab;
    public GameObject crabObj;
    public PlayerClawEntity clawEntity;
    public LifeBar myLifeBar;
    private Vector3 _initPos;
    private Quaternion _initRot;
    public float distanceBetwinCrabs = 1f;
    private float maxLifePoint = 2f;
    private float _life;
    private Turtel _turtel;
    private bool _pushed = false;
    [SerializeField] GameObject _surimiPartucul = null;

    //restpawn
    private int _nbSpawnPoint;
    [SerializeField]private float _distMarge = 0.5f;

    //Move
    [Header("Move")]
    public PlayerClawDirection clawDirection = PlayerClawDirection.North;
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 _moveDir = Vector2.zero;

    // Dash
    [Header("Dash")]
    private bool canBump = false;
    private Vector2 _dashDir;
    public float dashDuration = 0.12f;
    public float dashSpeed = 20f;

    private float _dashCountdown = 0f;
    private bool _isDashing = false;

    private bool _inSponge = false;

    // Parry
    [Header("Parry")]
    [SerializeField] private GameObject _bullParry = null;
    private bool _isParry = false;
    [SerializeField] private float _timeOfParry = 4f;
    private bool _isParryInfinit = false;
    private float _timeParryLeft = 4f;

    // Friction
    [Header("Friction")]
    public float friction = 30f;
    public float turnAroundFriction = 20f;


    // Orientation
    [Header("Orientation")]
    public bool inOrientation = false;
    private Vector2 _dir = Vector2.zero;    // vecteur direction du joueur

    // FX
    [Header("FX")]
    [SerializeField]private GameObject _fxAttaqueOnGarde = null;
    [SerializeField]private GameObject _fxGarde = null;            //ok
    [SerializeField] private GameObject _fxHit = null;             //ok
    [SerializeField] private GameObject _fxShildBreak = null;      //ok
    [SerializeField] private GameObject _fxWin = null;             //ok
    [SerializeField] private GameObject _fxWinLoop = null;         //ok
    [SerializeField] private GameObject _fxTakeDamage = null;      //ok
    [SerializeField] private GameObject _fxBump = null;            //
    [SerializeField] private GameObject _fxRespawn = null;         //

    [SerializeField] private GameObject _trailDash = null;         //ok


    //anim
    public Animator anim;

    // Debug
    [Header("Debug")]
    public int idCrab;
    public bool debugMode = false;
    public bool unlimitedParry = false;

    #endregion

    private void Awake()
    {
        _life = maxLifePoint;
        crabs[idCrab] = this;
        _initPos = transform.position;
        _initRot = crabObj.transform.rotation;
        _bullParry.SetActive(false);
        _timeParryLeft = _timeOfParry;
        if (unlimitedParry)
        {
            _isParryInfinit = true;
            _bullParry.SetActive(_isParry);
            _isParry = true;
        }
        if (_timeOfParry < 0f)
        {
            _isParryInfinit = true;
        }
    }

    private void Start()
    {
        anim.Play("Armature|Crab_Idle");

        Party.instance.entity.Add(gameObject);
        _UpdateModelOrient();
        _nbSpawnPoint = MapSetup.instance.SpawnPoints.Count;

        if (_trailDash != null)
        {
            _trailDash.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inOrientation)
        {
            if (!_isParry)
            {
                if (_isDashing)
                {
                    _UpdateDash();
                }
                _UpdatePos();

                _timeParryLeft += Time.fixedDeltaTime;
                if (_timeParryLeft >= _timeOfParry)
                {
                    _timeParryLeft = _timeOfParry;
                }
            }
            else if (!_isParryInfinit)
            {
                _timeParryLeft -= Time.fixedDeltaTime;
                if (_timeParryLeft <= 0f)
                {
                    Parry(false);

                    if (_fxShildBreak != null)
                    {
                        GameObject tmpObj = Instantiate(_fxShildBreak);
                        tmpObj.transform.position = transform.position;
                        Destroy(tmpObj, 3f);
                    }
                }
            }
        }
        else
        {
            Vector3 tmpPos = transform.position;
            tmpPos.x = _turtel.transform.position.x;
            tmpPos.y = _turtel.transform.position.y;
            transform.position = tmpPos;
        }
    }

    private void _UpdatePos()
    {
        Vector3 newPos = transform.position;
        //anim.Play("Armature|Crab_Idle", -1, 0f);
        if (!_isParry)
        {
            Vector2 vectTemp = new Vector2(clawEntity.transform.position.x - transform.position.x, clawEntity.transform.position.y - transform.position.y);
            if (_isDashing)
            {
                anim.SetBool("isDashing", true);

                if (_moveDir.x * vectTemp.x == 0 && _moveDir.y * vectTemp.y == 0)
                {
                    anim.SetInteger("direction", 0);
                }
                else if (_dashDir.x * vectTemp.x > 0 && _dashDir.y * vectTemp.y > 0)
                {
                    anim.SetInteger("direction", 1);
                    //anim.Play("Armature|Crab_dash_right");
                }
                else
                {
                    anim.SetInteger("direction", -1);

                    //anim.Play("Armature|Crab_dash_left");
                }
                newPos.x += _moveDir.x * Time.fixedDeltaTime * dashSpeed;
                newPos.y += _moveDir.y * Time.fixedDeltaTime * dashSpeed;
            }
            else
            {
                anim.SetBool("isDashing", false);

                if (_moveDir.x * vectTemp.x == 0 && _moveDir.y * vectTemp.y == 0)
                {
                    anim.SetInteger("direction", 0);
                }
                else if (_moveDir.x * vectTemp.x > 0 && _moveDir.y * vectTemp.y > 0)
                {
                    anim.SetInteger("direction", 1);

                    //anim.Play("Armature|Crab_Strafe_Right");
                }
                else
                {
                    anim.SetInteger("direction", -1);

                    anim.Play("Armature|Crab_Strafe_Left");
                }
                newPos.x += _moveDir.x * Time.fixedDeltaTime * moveSpeed;
                newPos.y += _moveDir.y * Time.fixedDeltaTime * moveSpeed;
            }

            if (!IsCollide(newPos) && !InBox(newPos, MapSetup.instance.colides))
            {
                foreach (PlayerEntity item in crabs)
                {
                    if (item != this)
                    {
                        
                        if (Vector3.Distance(newPos, item.gameObject.transform.position) < distanceBetwinCrabs)
                        {
                            if (_isDashing)
                            {
                                Debug.Log("Hey You");
                                _isDashing = false;
                                item.Parry(false);

                                if (canBump)
                                {
                                    item._pushed = true;
                                    item.ForceDash(_dashDir, _dashCountdown);
                                    item._pushed = false;
                                }
                            }

                            return;
                        }
                    }
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.forward, out hit))
                {
                    if (hit.transform.gameObject.tag == "Sponge")
                    {
                        Sponge sponge = hit.transform.gameObject.GetComponent<Sponge>();
                        if (sponge.Stop(newPos))
                        {
                            if (!sponge.Stop(transform.position))
                            {
                                if (_inSponge) return;
                                clawDirection = sponge.newOrientClaw(clawDirection);
                                _inSponge = true;
                                switch (clawDirection)
                                {
                                    case PlayerClawDirection.North:
                                        _dir = MapSetup.instance.vectorNorth;
                                        break;
                                    case PlayerClawDirection.South:
                                        _dir = MapSetup.instance.vectorSouth;
                                        break;
                                    case PlayerClawDirection.East:
                                        _dir = MapSetup.instance.vectorEast;
                                        break;
                                    case PlayerClawDirection.West:
                                        _dir = MapSetup.instance.vectorWest;
                                        break;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (sponge.Stop(transform.position))
                        {
                            return;
                        }

                        _inSponge = true;


                        Vector3 tmpPos = transform.position;
                        tmpPos.x = hit.transform.position.x;
                        tmpPos.y = hit.transform.position.y;
                        transform.position = tmpPos;

                        _dashDir = _dir;
                    }

                    transform.position = newPos;
                    return;
                }
                else
                {
                    _inSponge = false;
                }

                transform.position = newPos;
            }
            else
            {
                _moveDir = Vector3.zero;
            }
        }
    }

    #region Fonction Move

    public void Move(Vector2 dir)
    {
        _moveDir = (_dir.x * dir.x + _dir.y * dir.y) / (_dir.x * _dir.x + _dir.y * _dir.y) * _dir;
    }

    public void Dash()
    {
        anim.SetBool("isDashing", true);

        if (inOrientation || _isParry) return;

        if (_trailDash != null)
        {
            _trailDash.SetActive(true);
        }

        if (_moveDir.x > 0 && _moveDir.y> 0)
        {
            anim.Play("Armature|Crab_dash_right");
        }
        else
        {
            anim.Play("Armature|Crab_Strafe_Left");
        }

        _dashDir = _moveDir.normalized;
        _dashCountdown = dashDuration;
        _isDashing = true;
    }

    public void ForceDash(Vector2 dir, float durationLeft)
    {
        if (inOrientation) return;

        SetOrientation(dir);

        _dashDir = dir;
        _dashCountdown = durationLeft;
        //_dashCountdown = dashDuration;  // tmp for test
        _isDashing = true;
    }

    private void _UpdateDash()
    {
        if (!_isDashing) return;

        _dashCountdown -= Time.fixedDeltaTime;
        if (_dashCountdown >= 0f)
        {
            _moveDir = _dashDir;
        }
        else
        {
            if (_trailDash != null)
            {
                _trailDash.SetActive(false);
            }

            _isDashing = false;
        }
    }
    #endregion

    #region Fonction Rotate

    public void Activate(bool on = true)
    {
        if (_isParry) return;
        if (!on && _turtel == null) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            if (hit.transform.gameObject.tag == "Rotor")
            {
                _turtel = hit.transform.gameObject.GetComponent<Turtel>();

                if (_turtel)
                {

                    if (_turtel.GetImobility())
                    {
                        Vector3 tmpPos = transform.position;
                        tmpPos.x = hit.transform.position.x;
                        tmpPos.y = hit.transform.position.y;
                        transform.position = tmpPos;
                        inOrientation = !inOrientation;
                        _moveDir = Vector2.zero;

                        if (!inOrientation)
                        {
                            _turtel = null;
                        }
                    }
                }
            }
        }
    }

    public void Unactivate()
    {
        if (_isParry) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            if (hit.transform.gameObject.tag == "Rotor")
            {
                _turtel = hit.transform.gameObject.GetComponent<Turtel>();

                if (_turtel)
                {

                    if (_turtel.GetImobility())
                    {
                        Vector3 tmpPos = transform.position;
                        tmpPos.x = hit.transform.position.x;
                        tmpPos.y = hit.transform.position.y;
                        transform.position = tmpPos;
                        inOrientation = !inOrientation;
                        _moveDir = Vector2.zero;
                    }
                }
            }
        }
    }

    public void SetOrientation(Vector2 dir)
    {
        if (!_pushed)
        {
            if (!_turtel.GetImobility()) return;
        }
        if (dir.x > 0.1f)
        {
            if (dir.y > 0.1f)
            {
                clawDirection = PlayerClawDirection.North;
                crabObj.transform.rotation = Quaternion.Euler(-20, 310, 20);
            }
            else if (dir.y < -0.1f)
            {
                clawDirection = PlayerClawDirection.East;
                crabObj.transform.rotation = Quaternion.Euler(-25, 45, -25);
            }
        }
        else if (dir.x < -0.1f)
        {
            if (dir.y < -0.1f)
            {
                clawDirection = PlayerClawDirection.South;
                crabObj.transform.rotation = Quaternion.Euler(20, 130, -20);
            }
            else if (dir.y > 0.1f)
            {
                clawDirection = PlayerClawDirection.West;
                crabObj.transform.rotation = Quaternion.Euler(25, 220, 25);
            }
        }
        _UpdateModelOrient();
        if (!_pushed)
        {
            _turtel.Rotat(this);
        }
        clawEntity.Rotat(clawDirection);
    }

    private void _UpdateModelOrient()
    {
        switch (clawDirection)
        {
            case PlayerClawDirection.North:
                _dir = MapSetup.instance.vectorNorth;
                break;
            case PlayerClawDirection.South:
                _dir = MapSetup.instance.vectorSouth;
                break;
            case PlayerClawDirection.East:
                _dir = MapSetup.instance.vectorEast;
                break;
            case PlayerClawDirection.West:
                _dir = MapSetup.instance.vectorWest;
                break;
        }
    }

    #endregion 

    #region Fonction Collision

    private bool InBox(Vector3 _pos, List<BoxCollider> colides)
    {
        foreach (var item in colides)
        {
            if (item.bounds.Contains(_pos))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsCollide(Vector3 _pos)
    {
        if (Vector2.Distance(MapSetup.instance.transform.position, _pos) > MapSetup.instance.mapSize)
        {
            return true;
        }
        return false;
    }

    private bool ColideInDash(Vector3 _pos)
    {
        if (Vector2.Distance(MapSetup.instance.transform.position, _pos) > MapSetup.instance.mapSize)
        {
            return true;
        }
        return false;
    }

    public void Parry(bool isParry)
    {
        _isParry = isParry;
        _bullParry.SetActive(_isParry);
        _dashCountdown = 0f;
    }

    public bool GetIsParry()
    {
        return _isParry;
    }
    #endregion

    #region Fonction Life

    public bool TakeDamage(float _dammage, PlayerClawEntity howHit)
    {
        if (_isParry)
        {
            howHit.hitTipe = HitTipe.hitShild;
            if(_fxGarde != null)
            {
                GameObject tmpObj = Instantiate(_fxGarde);
                tmpObj.transform.position = transform.position;
                Destroy(tmpObj, 3f);
            }
            return false;
        }
        else
        {
            howHit.hitTipe = HitTipe.hitCrab;

            _life -= _dammage;
            if (_fxHit != null)
            {
                GameObject tmpObj = Instantiate(_fxHit);
                tmpObj.transform.position = transform.position;
                Destroy(tmpObj, 3f);
            }
            if (_fxTakeDamage != null)
            {
                GameObject tmpObj = Instantiate(_fxTakeDamage);
                tmpObj.transform.position = transform.position;
                Destroy(tmpObj, 3f);
            }
        }
        if (_life <= 0f)
        {
            AudioManager.instance.Play("Son_Choc_Crabe");
            _life = 0f;
            _UpdateLife();

            if (Random.Range(0f, 2f) > 1f)    // message du joueur
            {
                switch (howHit.myPlayer.idCrab)
                {
                    case 0:
                        AudioManager.instance.Play("Rouge_attaquant");
                        break;
                    case 1:
                        AudioManager.instance.Play("Bleu_attaquant");
                        break;
                    case 2:
                        AudioManager.instance.Play("Vert_attaquant");
                        break;
                    case 3:
                        AudioManager.instance.Play("Jaune_attaquant");
                        break;
                }
            }
            else                            // message du crab toucher
            {
                switch (idCrab)
                {
                    case 0:
                        AudioManager.instance.Play("Rouge_defenseur");
                        break;
                    case 1:
                        AudioManager.instance.Play("Bleu_defenseur");
                        break;
                    case 2:
                        AudioManager.instance.Play("Vert_defenseur");
                        break;
                    case 3:
                        AudioManager.instance.Play("Jaune_defenseur");
                        break;
                }
            }

            Kill();
            return true;
        }
        else
        {
            AudioManager.instance.Play("Son_Choc_Crabe");
            _UpdateLife();
        }
        return false;
    }

    private void _UpdateLife()
    {
        myLifeBar.SetLife(_life, maxLifePoint);
    }

    private void Kill()
    {
        CameraShaker.Instance.ShakeOnce(2f,2f,0.1f,1f);

        _isAlive = false;
        inOrientation = false;
        clawEntity.canMakeDamage = false;

        if (_surimiPartucul != null)
        {
            Destroy(Instantiate(_surimiPartucul, transform.position, Quaternion.Euler(0, 0, 0)), 2f);
        }
        allInCrab.SetActive(false);
        transform.position = new Vector3(100, -100, 100);
        
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);

        _life = maxLifePoint;
        _UpdateLife();
        if (_nbSpawnPoint < 0)
        {
            Debug.Log("no Spawnpoin");
            transform.position = _initPos;
        }
        else
        {
            transform.position = RespawnPoin();
        }

        if (_fxRespawn != null)
        {
            Destroy(Instantiate(_fxRespawn, transform.position, Quaternion.Euler(0, 0, 0)), 1f);
        }
        
        yield return new WaitForSeconds(1f);


        crabObj.transform.rotation = _initRot;

        allInCrab.SetActive(true);
        _isAlive = true;
    }

    private Vector3 RespawnPoin()
    {
        Vector3 tmpPos = _initPos;
        List<Vector3> tmpListSpawn = new List<Vector3>();

        for (int i = _nbSpawnPoint; i-- > 0;)
        {
            tmpListSpawn.Add(MapSetup.instance.SpawnPoints[i].transform.position);
        }

        for (int i = 4; i--> 0;)
        {
            if (crabs[i] != this)
            {
                for (int j = _nbSpawnPoint; j--> 0;)
                {
                    if (Vector3.Distance(crabs[i].transform.position, MapSetup.instance.SpawnPoints[j].transform.position) < _distMarge)
                    {
                        tmpListSpawn.Remove(MapSetup.instance.SpawnPoints[j].transform.position);
                    }
                }
            }
        }

        return tmpListSpawn[Random.Range(0, tmpListSpawn.Count-1)];
    }
    #endregion

    public void YouWin()
    {

        if (_fxWin != null)
        {
            GameObject tmpObj = Instantiate(_fxWin);
            tmpObj.transform.position = transform.position;
            Destroy(tmpObj, 3f);
        }
        if (_fxWinLoop != null)
        {
            GameObject tmpObj = Instantiate(_fxWinLoop);
            tmpObj.transform.position = transform.position;
        }
    }

    private void OnGUI()
    {
        if (!debugMode) { return; }

        GUILayout.BeginVertical();
        for (int i = 0; i < idCrab; i++)
        {
            GUILayout.Label("");
        }
        //GUILayout.Label(" _isdash : " + _isDashing);
        GUILayout.EndVertical();
    }
}
