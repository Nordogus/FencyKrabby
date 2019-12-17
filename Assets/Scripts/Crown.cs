using UnityEngine;

// camille

public class Crown : MonoBehaviour
{
    public static Crown instance;

    [SerializeField] private GameObject _sprit = null;
    [SerializeField] private float _speed = 1f;
    private GameObject _OldObjToFolo;
    private GameObject _objToFolo;
    private Vector3 _upPos = new Vector3(0, 1.6f, -1);

    [SerializeField] private GameObject _fxCrownPop = null;


    private float ratio = 0f;

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

        Hide();
    }

    private void Update()
    {
        if (_objToFolo == null)
        {
            return;
        }
        if (_OldObjToFolo != null)
        {
            transform.position = Vector3.Lerp(_OldObjToFolo.transform.position + _upPos, _objToFolo.transform.position + _upPos, ratio);
            ratio += Time.deltaTime * _speed;
        }
        else
        {
            transform.position = _objToFolo.transform.position + _upPos;
        }
    }

    public void Hide()
    {
        _sprit.SetActive(false);
    }

    public void Show()
    {
        _sprit.SetActive(true);

        if (_fxCrownPop != null)
        {
            GameObject tmpObj = Instantiate(_fxCrownPop);
            tmpObj.transform.position = _objToFolo.transform.position + _upPos;
            Destroy(tmpObj, 3f);
        }
    }

    public void SetObjectToFolo(GameObject _obj)
    {
        if (_obj == _objToFolo) return;

        if (_obj == null)
        {
            Hide();
            _OldObjToFolo = null;
            _objToFolo = null;
        }
        else
        {

            if (_objToFolo != null)
            {
                _OldObjToFolo = _objToFolo;
            }

            _objToFolo = _obj;
            ratio = 0f;

            Show();
        }

    }
}
