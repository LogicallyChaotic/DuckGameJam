using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DuckSpawner : MonoBehaviour
{
    [SerializeField] private List<Duck_controller> _duckPrefabs;
    [SerializeField] private List<Color> _DuckColours;
    [SerializeField] private int _startingDucksPooled;
    [SerializeField] private int _amountofDucksShownAtStart;
    [SerializeField] private Animator _ducktective;
    [Header("UI")] [SerializeField] 
    private Image _colourIndicator;
    [SerializeField] private Image _shapeIndicator;
    
    private int _currentNumOfDucksTotal;
    private Duck_controller _criminalDuck;
    private CircleCollider2D _SpawnArea;
    private float _spawnAreaRadius;
    private List<Duck_controller> _duckPool = new();
    private List<Duck_controller> _activeDucks = new();
    private Dictionary<string, Duck_controller> _ducksActive = new();


    public void Start()
    {
        _SpawnArea = GetComponent<CircleCollider2D>();
        _spawnAreaRadius = _SpawnArea.radius;
        _currentNumOfDucksTotal = _amountofDucksShownAtStart;
        for (int i = 0; i < _startingDucksPooled; i++)
        {
            Duck_controller tempDuckPooled = SpawnDuck();
            _duckPool.Add(tempDuckPooled);
        }

        StartRound(_amountofDucksShownAtStart);
    }

    private Duck_controller SpawnDuck()
    {
        Duck_controller _currentDuckTemp = _duckPrefabs[Random.Range(0, _duckPrefabs.Count)];
        Duck_controller tempDuckSpawned = Instantiate(_currentDuckTemp, transform.position
                                                                        + new Vector3(Random.Range(0, _spawnAreaRadius),
                                                                            Random.Range(0, _spawnAreaRadius),
                                                                            0), Quaternion.identity);
        tempDuckSpawned.gameObject.SetActive(false);
        tempDuckSpawned.SetDuckSpawner(this);
        _duckPool.Add(tempDuckSpawned);
        return tempDuckSpawned;
    }

    private void StartRound(int _amountOfDucksToSpawn)
    {
        _ducksActive.Clear();
        _activeDucks.Clear();
        for (int i = 0; i < _amountOfDucksToSpawn; i++)
        {
            if (!SetUpDucks())
            {
                SetUpDucks();
            }
        }

        int tempIndexOfCriminalDuck = Random.Range(0, _ducksActive.Count);
        _criminalDuck = _ducksActive.ElementAt(tempIndexOfCriminalDuck).Value;
        _colourIndicator.color = _criminalDuck.ReturnColor();
        _shapeIndicator.sprite = _criminalDuck.ReturnSprite();
    }

    private bool SetUpDucks()
    {

        Duck_controller tempDuck = GetPooledDuck();
        _duckPool.Remove(tempDuck);
        Color tempCol = _DuckColours[Random.Range(0, _DuckColours.Count)];
        tempDuck.SetDuckColor(tempCol);
        if (_ducksActive.ContainsKey(tempCol.ToString() + tempDuck._duckType.ToString()))
        {
            return false;
        }
        else
        {
            _ducksActive.Add(tempCol.ToString() + tempDuck._duckType.ToString(), tempDuck);
            tempDuck.gameObject.SetActive(true);
            _activeDucks.Add(tempDuck);
            return true;
        }
    }

    private Duck_controller GetPooledDuck()
    {
        if (_duckPool.Count != 0)
        {
            return _duckPool[Random.Range(0, _duckPool.Count)];
        }

        return SpawnDuck();
    }

    public void DeactivateDucks(Duck_controller duckToRemove)
    {
        _activeDucks.Remove(duckToRemove);
        duckToRemove.gameObject.SetActive(false);

        if (_criminalDuck == duckToRemove)
        {
            //TODO:success
            Debug.Log("Success");
            _ducktective.SetTrigger("win");

            foreach (var duckActive in _activeDucks)
            {
                duckActive.gameObject.SetActive(false);
                _duckPool.Add(duckActive);
            }

            _currentNumOfDucksTotal++;
            StartRound(_currentNumOfDucksTotal);
        }
        else
        {
            //TODO:fail
            Debug.Log("Game Over");

        }
    }
}