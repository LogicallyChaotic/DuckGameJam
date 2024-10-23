using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Duck_controller : MonoBehaviour
{
    public DuckType _duckType;
    [SerializeField] private float _duckForceMinimum;
    [SerializeField] private float _duckForceMaximum;
    [SerializeField] private float _maxWaitTimeBeforeMoved;
    [SerializeField] private GameObject _duckBody;
    private SpriteRenderer _duckSpriteRend;
    private DuckSpawner _duckSpawner;
    private Animator _duckAnimator;
    private Rigidbody2D _duckRigidbody;
    private Vector3 _currentDuckForce;
    private bool _ableToMoveDuck = true;
    private bool _mousedOver = false;
    private Vector3 _prevLocation;
    private void OnEnable()
    {
        _duckAnimator.SetBool("Move", true);
        _duckAnimator.ResetTrigger("duckCaught");
        _mousedOver = false;
        _ableToMoveDuck = true;
    }

    private void Awake()
    {
        _duckSpriteRend = _duckBody.GetComponent<SpriteRenderer>();
        _duckRigidbody = GetComponent<Rigidbody2D>();
        _duckAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        DuckDirectionCalibration();
    }

    private void FixedUpdate()
    {
        if (_ableToMoveDuck)
        {
            StartCoroutine(c_MoveDuck());
        }
    }

    public Color ReturnColor()
    {
        return _duckSpriteRend.color;
    }
    public Sprite ReturnSprite()
    {
        return _duckSpriteRend.sprite;
    }
    public void SetDuckColor(Color _colour)
    {
        _duckSpriteRend.color = _colour;
    }
    
    private void RemoveDuck()
    {
        _duckSpawner.DeactivateDucks(this);
    }

    private void OnMouseEnter()
    {
        _mousedOver = true;
        _duckRigidbody.velocity= Vector3.zero;
        _duckAnimator.SetBool("Move", false);
    }

    private void OnMouseDown()
    {
        DuckCaught();
    }

    private void OnMouseExit()
    {
        _mousedOver = false;
    }
    private void DuckCaught()
    {
        _duckAnimator.SetTrigger("duckCaught");
        _mousedOver = false;
    }
    private void DuckDirectionCalibration()
    {
        if (_duckRigidbody.velocity.x > 0)
        {
            //right
            _duckBody.transform.rotation= Quaternion.Euler(Vector3.zero);
        }
        else if (_duckRigidbody.velocity.x < 0)
        {
            //left
            _duckBody.transform.rotation= Quaternion.Euler(new Vector3(0,180,0));
        }

    }
    private IEnumerator c_MoveDuck()
    {
        if (_ableToMoveDuck && !_mousedOver)
        {
            _duckAnimator.SetBool("Move", true);

            _ableToMoveDuck = false;
            _currentDuckForce = new Vector3(Random.Range(_duckForceMinimum, _duckForceMaximum),
                Random.Range(_duckForceMinimum, _duckForceMaximum),
                Random.Range(_duckForceMinimum, _duckForceMaximum));
            _duckRigidbody.AddForce(_currentDuckForce);
            yield return new WaitForSeconds(Random.Range(0, _maxWaitTimeBeforeMoved));
            _ableToMoveDuck = true;
        }
        
    }

    public void SetDuckSpawner(DuckSpawner duckSpawner)
    {
        _duckSpawner = duckSpawner;
    }
}

public enum DuckType
{
    eRound,
    eTall,
    eShort,
    eSmall
}