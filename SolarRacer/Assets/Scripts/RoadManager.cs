using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{

    //Road tiles
    [SerializeField]
    private List<GameObject> _roadTiles = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _roadTilesWithJump = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _innerCityTiles = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _innerCityTilesWithJump = new List<GameObject>();
    [SerializeField]
    private GameObject _startingTile;
    [SerializeField]
    private GameObject _finalTile;
    //Obstacle tiles
    [SerializeField]
    private List<GameObject> _obstacleTiles = new List<GameObject>();
    //Dark zones tiles
    //Amount of each tile in the object pool
    [SerializeField]
    private int _amountOfTilesInPool = 3;
    //Road Speed
    [SerializeField]
    private float _roadSpeed = 50.0f;
    //Road Pieces On Screen
    [SerializeField]
    private int _roadPiecesOnScreen = 5;
    //Length of the road in tiles
    [SerializeField]
    private int _roadLength = 200;

    [SerializeField]
    [Range(0f, 1f)]
    private float _probabilityOfDarkZoneInTile = 0.0f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _percentageOuter_InnerCity = 0.5f;

    [Header("Pick-Ups & Collectables")]
    [SerializeField]
    private List<GameObject> _collectables = new List<GameObject>();
    [SerializeField]
    private int _numberOfCollectablesToPoints = 5;
    [SerializeField]
    private float _multiplierNumberOfCollectablesToSpawn = 1f;
    [SerializeField]
    private int _pointsPerCollectableGroup = 10000;
    [SerializeField]
    private int _pointsPerPickUp = 10;

    private int[] _rngArray;

    public void Awake()
    {
        _rngArray = new int[_roadLength];
        GenerateRNG();
        StaticDataContainer._pointsPerCollectableGroup = _pointsPerCollectableGroup;
        StaticDataContainer._pointsPerPickUp = _pointsPerPickUp;
    }

    public List<GameObject> GetRoadTiles()
    {
        return _roadTiles;
    }

    public List<GameObject> GetRoadTilesWithJump()
    {
        return _roadTilesWithJump;
    }

    public List<GameObject> GetInnerCityRoadTiles()
    {
        return _innerCityTiles;
    }

    public List<GameObject> GetInnerCityRoadTilesWithJump()
    {
        return _innerCityTilesWithJump;
    }

    public List<GameObject> GetCollectablesList()
    {
        return _collectables;
    }

    public int GetNumberOfCollectablesToPoints()
    {
        return _numberOfCollectablesToPoints;
    }

    public float GetMultiplierNumberOfCollectablesToSpawn()
    {
        return _multiplierNumberOfCollectablesToSpawn;
    }

    public List<GameObject> GetObstacleTiles()
    {
        return _obstacleTiles;
    }

    public int GetAmountOfTilesInPool()
    {
        return _amountOfTilesInPool;
    }

    public int GetRoadPiecesOnScreen()
    {
        return _roadPiecesOnScreen;
    }

    public float GetRoadSpeed()
    {
        return _roadSpeed;
    }

    public int GetRoadLength()
    {
        return _roadLength;
    }

    public GameObject GetFinalTile()
    {
        return _finalTile;
    }

    public GameObject GetStartingTile()
    {
        return _startingTile;
    }

    private void GenerateRNG()
    {
        int seed = Random.Range(0, 5000);
        for (int i = 0; i < _rngArray.Length; i++)
        {
            Random.InitState(seed);
            int num = Random.Range(0, 5000);
            _rngArray[i] = num;
            seed = num;
        }

    }

    public int GetRandomSeed(int pTileCount)
    {
        return _rngArray[pTileCount];
    }

    public int GenerateRandomTile(List<Tile> pTilePool, int pTileCount, int maxIndexExclusive = -1)
    {
        Random.InitState(GetRandomSeed(pTileCount));
        int randomTile;
        if (maxIndexExclusive == -1)
        {
            randomTile = Random.Range(0, pTilePool.Count);

            while (pTilePool[randomTile].GO.activeSelf) //potencial infinite loop if less tiles than tiles_in_screen
            {
                randomTile = Random.Range(0, pTilePool.Count);
            }
        }
        else
        {
            randomTile = Random.Range(0, maxIndexExclusive);
            while (pTilePool[randomTile].GO.activeSelf) //potencial infinite loop if less tiles than tiles_in_screen
            {
                randomTile = Random.Range(0, maxIndexExclusive);
            }
        }

        return randomTile;
    }

    public float GetDarkZoneProb()
    {
        return _probabilityOfDarkZoneInTile;
    }

    public float GetOuterInnerPercentaje()
    {
        return _percentageOuter_InnerCity;
    }

    public float PointsPerPickUp
    {
        get
        {
            return _pointsPerPickUp;
        }
    }

    public float PoinstPerCollectableGroup
    {
        get
        {
            return _pointsPerCollectableGroup;
        }
    }

    public void ResetCurve()
    {
        gameObject.GetComponent<CurveManager>().ResetCurvature();
    }
}

