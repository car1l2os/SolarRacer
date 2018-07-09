using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{

    //The player on this road
    [SerializeField]
    private GameObject _player;

    //Values from parent
    private List<GameObject> _roadTiles = new List<GameObject>();
    private List<GameObject> _roadTilesWithJump = new List<GameObject>();
    private List<GameObject> _innerCityRoadTiles = new List<GameObject>();
    private List<GameObject> _innerCityRoadTilesWithJump = new List<GameObject>();
    private List<GameObject> _obstacleTiles = new List<GameObject>();
    private List<GameObject> _clouds = new List<GameObject>();
    private List<GameObject> _collectables = new List<GameObject>();
    private int _amountOfTilesInPool = 0;
    private int _roadPiecesOnScreen = 0;
    private float _roadSpeed = 0;

    //Private values
    private List<Tile> _roadTilePool = new List<Tile>();
    private List<Tile> _innerCityRoadTilesPool = new List<Tile>();
    private List<Tile> _activeRoadTiles = new List<Tile>();
    private RoadManager _roadManager;
    [SerializeField] //debug code 
    private int _tileCount = 0;
    private int _outerTiles = 0;
    private List<GameObject> _objectsInRoad = new List<GameObject>();
    private float _speedModifier = 1f;
    [SerializeField]//debug code  
    private float _powerSpeedModifier = 0f;
    private bool _finishSpawned = false;
    private bool _finishReached = false;
    private Tile _finalTile;
    private Tile _startingTile;
    private float _penaltyTimer = 0.0f;
    private bool _onAlteredState = false;
    private int _tilesToCollectable = 0;
    private Queue<GameObject> _collectablesPool = new Queue<GameObject>();
    public bool _powerVelocityLocked = false;

    //Temp values
    private GameObject _tempRoadTile;

    void Start()
    {
        _speedModifier = 0.0f;
        GetValues();
        PutRoadTilesInObjectPool();
        PutInnerRoadTilesInObjectPool();
        PutRoadTilesWithJumpInObjectPool();
        PutInnerRoadTilesWithJumpInObjectPool();
        PutCollectablesInPool();
        AddObstaclesToTiles(); //dark zones now in obstacles
        GenerateStartingRoad();
    }

    private void Update()
    {
        RemoveTileBehindPlayer();
        CheckAlteredStates();
        CheckFinished();
    }

    private void GetValues()
    {
        _roadManager = GetComponentInParent<RoadManager>();

        _roadTiles = _roadManager.GetRoadTiles();
        _roadTilesWithJump = _roadManager.GetRoadTilesWithJump();
        _innerCityRoadTiles = _roadManager.GetInnerCityRoadTiles();
        _innerCityRoadTilesWithJump = _roadManager.GetInnerCityRoadTilesWithJump();
        _amountOfTilesInPool = _roadManager.GetAmountOfTilesInPool();
        _obstacleTiles = _roadManager.GetObstacleTiles();
        _roadPiecesOnScreen = _roadManager.GetRoadPiecesOnScreen();
        _roadSpeed = _roadManager.GetRoadSpeed();
        _outerTiles = (int)(_roadManager.GetRoadLength() * _roadManager.GetOuterInnerPercentaje());
        _collectables = _roadManager.GetCollectablesList();

        if (_roadManager.GetNumberOfCollectablesToPoints() != 0)
            _tilesToCollectable = _roadManager.GetRoadLength() / (int)(_roadManager.GetNumberOfCollectablesToPoints() * _roadManager.GetMultiplierNumberOfCollectablesToSpawn());
    }

    private void PutRoadTilesInObjectPool()
    {
        for (int i = 0; i < _roadTiles.Count; i++)
        {
            for (int j = 0; j < _amountOfTilesInPool; j++)
            {
                _tempRoadTile = Instantiate(_roadTiles[i], transform.position, new Quaternion(0, 180, 0, 0));
                _tempRoadTile.gameObject.SetActive(false);
                _roadTilePool.Add(new Tile(_tempRoadTile));
            }
        }
    }

    private void PutRoadTilesWithJumpInObjectPool()
    {
        for (int i = 0; i < _roadTilesWithJump.Count; i++)
        {
            for (int j = 0; j < _amountOfTilesInPool; j++)
            {
                _tempRoadTile = Instantiate(_roadTilesWithJump[i], transform.position, new Quaternion(0, 180, 0, 0));
                _tempRoadTile.gameObject.SetActive(false);
                _roadTilePool.Add(new Tile(_tempRoadTile, true));
            }
        }
    }

    private void PutInnerRoadTilesInObjectPool()
    {
        for (int i = 0; i < _innerCityRoadTiles.Count; i++)
        {
            for (int j = 0; j < _amountOfTilesInPool; j++)
            {
                _tempRoadTile = Instantiate(_innerCityRoadTiles[i], transform.position, new Quaternion(0, 180, 0, 0));
                _tempRoadTile.gameObject.SetActive(false);
                _innerCityRoadTilesPool.Add(new Tile(_tempRoadTile));
            }
        }
    }

    private void PutInnerRoadTilesWithJumpInObjectPool()
    {
        for (int i = 0; i < _innerCityRoadTilesWithJump.Count; i++)
        {
            for (int j = 0; j < _amountOfTilesInPool; j++)
            {
                _tempRoadTile = Instantiate(_innerCityRoadTilesWithJump[i], transform.position, new Quaternion(0, 180, 0, 0));
                _tempRoadTile.gameObject.SetActive(false);
                _innerCityRoadTilesPool.Add(new Tile(_tempRoadTile, true));
            }
        }
    }

    private void PutCollectablesInPool()
    {
        int numberToPool = (int)(_roadManager.GetNumberOfCollectablesToPoints() * _roadManager.GetMultiplierNumberOfCollectablesToSpawn());
        GameObject tmp;

        for (int i = 0; i < numberToPool; i++)
        {
            tmp = Instantiate(_collectables[0], Vector3.zero, Quaternion.identity);
            tmp.SetActive(false);
            _collectablesPool.Enqueue(tmp);
        }
    }


    private void AddObstaclesToTiles()
    {
        GameObject tempObstacle;
        //Outer tiles
        for (int i = 0; i < _roadTilePool.Count; i++)
        {
            if (_roadTilePool[i].JumpTile == false)
            {
                Random.InitState(_roadManager.GetRandomSeed(i));
                int rndObstacle = Random.Range(0, _obstacleTiles.Count);
                tempObstacle = Instantiate(_obstacleTiles[rndObstacle], _roadTilePool[i].GO.transform.position, new Quaternion(0, 180, 0, 0));
                tempObstacle.transform.parent = _roadTilePool[i].GO.transform;
                _roadTilePool[i].ObstaclesGroup = tempObstacle;
                _roadTilePool[i].GroundObstacles = tempObstacle.transform.GetChild(0).gameObject; //important to check
                _roadTilePool[i].Clouds = tempObstacle.transform.GetChild(1).gameObject;
            }
        }

        //Inner tiles
        for (int i = 0; i < _innerCityRoadTilesPool.Count; i++)
        {
            if (_innerCityRoadTilesPool[i].JumpTile == false)
            {
                Random.InitState(_roadManager.GetRandomSeed(i));
                int rndObstacle = Random.Range(0, _obstacleTiles.Count);
                tempObstacle = Instantiate(_obstacleTiles[rndObstacle], _innerCityRoadTilesPool[i].GO.transform.position, new Quaternion(0, 180, 0, 0));
                tempObstacle.transform.parent = _innerCityRoadTilesPool[i].GO.transform;
                _innerCityRoadTilesPool[i].ObstaclesGroup = tempObstacle;
                _innerCityRoadTilesPool[i].GroundObstacles = tempObstacle.transform.GetChild(0).gameObject;
                _innerCityRoadTilesPool[i].Clouds = tempObstacle.transform.GetChild(1).gameObject;

            }
        }
    }

    private void GenerateStartingRoad()
    {
        //Cleaner and usign SpawnTileAtEndOfRoad()
        Vector3 roadStart = _player.transform.position - new Vector3(0, 0.5f, 0);
        _startingTile = new Tile(Instantiate(_roadManager.GetStartingTile(), transform.position, new Quaternion(0, 180, 0, 0)));
        SpawnTile(_startingTile, _activeRoadTiles, roadStart);
        _startingTile.GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;

        //special case for tile 1 == can not be a jump tile. 
        int randomRoadFromPool = _roadManager.GenerateRandomTile(_roadTilePool, _tileCount, _roadTilePool.Count - _roadTilesWithJump.Count - _innerCityRoadTilesWithJump.Count);

        SpawnTile(_roadTilePool[randomRoadFromPool], _activeRoadTiles, _activeRoadTiles[_activeRoadTiles.Count - 1].GO.transform.position - new Vector3(_roadTilePool[randomRoadFromPool].GO.transform.localScale.x - _roadSpeed * _speedModifier * Time.deltaTime, 0, 0));
        _roadTilePool[randomRoadFromPool].GO.SetActive(true);
        _roadTilePool[randomRoadFromPool].GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;
        _tileCount++;

        for (int i = 1; i < _roadPiecesOnScreen; i++)
        {
            SpawnTileAtEndOfRoad();
        }
    }

    private void RemoveTileBehindPlayer()
    {
        if (_activeRoadTiles[0].GO.transform.position.x >= _activeRoadTiles[0].GO.transform.localScale.x) //Broke at some point without any changes. Only breaks every once in a while.
        {
            _activeRoadTiles[0].GO.transform.position = Vector3.zero;

            if (_activeRoadTiles[0].GroundObstacles != null)
            {
                Renderer[] _makeVisibleAgain = _activeRoadTiles[0].GroundObstacles.GetComponentsInChildren<Renderer>();
                if (_makeVisibleAgain != null)
                {
                    for (int i = 0; i < _makeVisibleAgain.Length; i++)
                    {
                        _makeVisibleAgain[i].enabled = true;
                    }
                }
            }

            _activeRoadTiles[0].GO.SetActive(false);

            //Debug.Log(_activeRoadTiles[0].GO.name);

            _activeRoadTiles.RemoveAt(0);

            _activeRoadTiles.TrimExcess();
        }
        else
        {
            return;
        }
        SpawnTileAtEndOfRoad();
        CheckAndChangeTileStyle();
        PlaceCollectables();
    }

    private void CheckAlteredStates()
    {
        if (_penaltyTimer > 0.0f)
            _penaltyTimer -= Time.deltaTime;
        else if (_onAlteredState)
        {
            SpeedModifier = 1.0f;
            _onAlteredState = false;
        }
    }

    private void SpawnTileAtEndOfRoad()
    {
        if (_finishSpawned)
        {
            return;
        }

        if (_tileCount >= _roadManager.GetRoadLength())
        {
            _finalTile = new Tile(Instantiate(_roadManager.GetFinalTile()));
            SpawnTile(_finalTile, _activeRoadTiles, _activeRoadTiles[_activeRoadTiles.Count - 1].GO.transform.position - new Vector3(_finalTile.GO.transform.localScale.x - _roadSpeed * _speedModifier * _powerSpeedModifier * Time.deltaTime, 0, 0));
            _finalTile.GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;
            _activeRoadTiles.Add(_finalTile);
            _finishSpawned = true;
            return;
        }

        //if previous tile has been a jump no obstacles 
        if (_activeRoadTiles[_activeRoadTiles.Count - 1].JumpTile == false)
        {
            int randomRoadFromPool = _roadManager.GenerateRandomTile(_roadTilePool, _tileCount);

            SpawnTile(_roadTilePool[randomRoadFromPool], _activeRoadTiles, _activeRoadTiles[_activeRoadTiles.Count - 1].GO.transform.position - new Vector3(_roadTilePool[randomRoadFromPool].GO.transform.localScale.x - _roadSpeed * _speedModifier * _powerSpeedModifier * Time.deltaTime, 0, 0));
            _roadTilePool[randomRoadFromPool].GO.SetActive(true);
            if (_roadTilePool[randomRoadFromPool].JumpTile == false)
            {
                _roadTilePool[randomRoadFromPool].ObstaclesGroup.SetActive(true);
                if (_roadTilePool[randomRoadFromPool].Collectables != null)
                    _roadTilePool[randomRoadFromPool].Collectables.SetActive(true);
            }
            _roadTilePool[randomRoadFromPool].GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;

        }
        else
        {
            int randomRoadFromPool = _roadManager.GenerateRandomTile(_roadTilePool, _tileCount, _roadTilePool.Count - _roadTilesWithJump.Count - _innerCityRoadTilesWithJump.Count);

            SpawnTile(_roadTilePool[randomRoadFromPool], _activeRoadTiles, _activeRoadTiles[_activeRoadTiles.Count - 1].GO.transform.position - new Vector3(_roadTilePool[randomRoadFromPool].GO.transform.localScale.x - _roadSpeed * _speedModifier * _powerSpeedModifier * Time.deltaTime, 0, 0));
            _roadTilePool[randomRoadFromPool].GO.SetActive(true);
            if (_roadTilePool[randomRoadFromPool].JumpTile == false) //jump tiles don't have obstacles
            {
                _roadTilePool[randomRoadFromPool].ObstaclesGroup.SetActive(false);
                if (_roadTilePool[randomRoadFromPool].Collectables != null)
                    _roadTilePool[randomRoadFromPool].Collectables.SetActive(false);
            }
            _roadTilePool[randomRoadFromPool].GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;
        }

        Animator[] tmp = _activeRoadTiles[_activeRoadTiles.Count - 1].GO.GetComponentsInChildren<Animator>();
        if (tmp != null)
        {
            foreach (Animator anim in tmp)
            {
                anim.SetInteger("State", 0);
            }
        }

        _tileCount++;
    }

    private void CheckFinished()
    {
        if (!_finishSpawned)
        {
            return;
        }

        if (_player.transform.position.x <= _finalTile.GO.transform.position.x)
        {
            if (StaticDataContainer._firstInFinish == 0)
            {
                StaticDataContainer._firstInFinish = _player.GetComponent<VehicleManager>().Player;
                AudioManager tmp = FindObjectOfType<AudioManager>();
                tmp.StopEverything();
                tmp.Play("Victory");
            }
            _finishReached = true;
            RoadSpeed = 0;
            //_roadManager.ResetCurve();
        }
    }

    private void SpawnTile(Tile pTile, List<Tile> pActiveTiles, Vector3 pPostition)
    {
        pTile.GO.transform.position = pPostition;
        pActiveTiles.Add(pTile);
    }

    public float RoadSpeed
    {
        get
        {
            return _roadSpeed;
        }
        set
        {
            _roadSpeed = value;
            foreach (Tile tile in _activeRoadTiles) //added - Carlos
            {
                tile.GO.GetComponent<Movement>().Speed = value;
            }
        }
    }
    public float SpeedModifier
    {
        get
        {
            return _speedModifier;
        }
        set
        {
            if (value < _speedModifier || value == 1.0f)
            {
                _speedModifier = value;
                foreach (Tile tile in _activeRoadTiles)
                {
                    tile.GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;
                }
            }
        }
    }

    public float PowerSpeedModifier
    {
        get
        {
            return _powerSpeedModifier;
        }
        set
        {
            if (_powerVelocityLocked == false)
            {
                _powerSpeedModifier = value;
                foreach (Tile tile in _activeRoadTiles)
                {
                    tile.GO.GetComponent<Movement>().Speed = _roadSpeed * _speedModifier * _powerSpeedModifier;
                }
            }
        }
    }

    public float ModifiedSpeed
    {
        get
        {
            return _roadSpeed * _speedModifier * _powerSpeedModifier;
        }
    }

    public void SetVelocityPenaltyForTime(float penalty, float time)
    {
        SpeedModifier = penalty;
        _penaltyTimer = time;
        _onAlteredState = true;
    }

    public float PenaltyTimer
    {
        get
        {
            return _penaltyTimer;
        }
    }

    public bool FinishReached
    {
        get
        {
            return _finishReached;
        }
        set
        {
            _finishReached = true;
        }
    }

    private void CheckAndChangeTileStyle()
    {
        if (_tileCount == _outerTiles)
            ChangeTileStyle();
    }

    private void PlaceCollectables()
    {
        if (_tileCount % _tilesToCollectable == 0)
        {
            GameObject tmp;
            GameObject tmp_lastTile;
            if (_collectablesPool.Count != 0)
            {
                tmp_lastTile = _activeRoadTiles[_activeRoadTiles.Count - 1].GO;
                tmp = _collectablesPool.Dequeue();
                tmp.SetActive(true);
                _activeRoadTiles[_activeRoadTiles.Count - 1].Collectables = tmp;

                Vector3 pos = LookForFreePositionInTile(_activeRoadTiles[_activeRoadTiles.Count - 1]);
                if (pos != Vector3.zero)
                    tmp.transform.position = new Vector3(pos.x, 2f, pos.z);
            }
        }
    }

    private void ChangeTileStyle()
    {
        _roadTilePool.Clear();
        _roadTilePool = _innerCityRoadTilesPool;
    }

    private Vector3 LookForFreePositionInTile(Tile tile)
    {
        Transform[] lanes = _player.GetComponent<VehicleManager>().Lanes;
        Vector3 candidatePosition = new Vector3(0, 0, 0);

        int[] laneOrder = RandomLaneOrder();

        foreach (int lane in laneOrder)
        {
            candidatePosition = new Vector3(tile.GO.transform.position.x, 1f, lanes[lane].position.z); //temporal

            if (tile.GroundObstacles != null)
            {
                BoxCollider collider = tile.GroundObstacles.GetComponentInChildren<BoxCollider>();

                if (collider != null && collider.bounds.Contains(candidatePosition) == false) //lanzar rayos para asegurar minima distanacia
                {
                    RaycastHit hit, hit2;
                    bool negativeXAxis = Physics.Raycast(candidatePosition, Vector3.left, out hit, 5f);
                    bool positiveXAxis = Physics.Raycast(candidatePosition, Vector3.right, out hit2, 5f);
                    Debug.DrawRay(candidatePosition, Vector3.left * 5f, Color.red, 30f);
                    Debug.DrawRay(candidatePosition, Vector3.right * 5f, Color.red, 30f);

                    if (negativeXAxis == false && positiveXAxis == false)
                        return candidatePosition;
                }
            }
        }
        return candidatePosition;
    }

    private int[] RandomLaneOrder()
    {
        List<int> order = new List<int>();
        for (int i = 0; i < 3; i++)
            order.Add(i);


        int[] to_return = new int[3];
        int index = -1;
        for (int i = 0; i < to_return.Length; i++)
        {
            index = Random.Range(0, order.Count - 1);
            to_return[i] = order[index];
            order.RemoveAt(index);
        }

        return to_return;
    }

    public int CollectablesToPoints
    {
        get
        {
            return _roadManager.GetNumberOfCollectablesToPoints();
        }
    }

    public float PointsPerPickUp
    {
        get
        {
            return _roadManager.PointsPerPickUp;
        }
    }

    public float PoinstPerCollectableGroup
    {
        get
        {
            return _roadManager.PoinstPerCollectableGroup;
        }
    }


}

public class Tile
{
    private GameObject m_tileGameObject;
    private GameObject m_ObstaclesGroup;
    private GameObject m_groundObstacles;
    private GameObject m_collectables;
    private GameObject m_clouds;
    private bool m_jumpTile;

    public Tile(GameObject _go, bool _withJump = false)
    {
        m_tileGameObject = _go;
        m_groundObstacles = null;
        m_collectables = null;
        m_jumpTile = _withJump;
        m_ObstaclesGroup = null;
    }

    public GameObject GO
    {
        get
        {
            return m_tileGameObject;
        }
        set
        {
            m_tileGameObject = value;
        }
    }
    public GameObject ObstaclesGroup
    {
        set
        {
            m_ObstaclesGroup = value;
        }
        get
        {
            return m_ObstaclesGroup;
        }
    }

    public GameObject GroundObstacles
    {
        get
        {
            return m_groundObstacles;
        }
        set
        {
            m_groundObstacles = value;
            //value.gameObject.transform.parent = m_tileGameObject.transform;
        }
    }
    public GameObject Collectables
    {
        get
        {
            return m_collectables;
        }
        set
        {
            m_collectables = value;
            value.gameObject.transform.parent = m_tileGameObject.transform;
        }
    }
    public GameObject Clouds
    {
        get
        {
            return m_clouds;
        }
        set
        {
            m_clouds = value;
            //m_clouds.transform.parent = m_tileGameObject.transform;
        }
    }

    public bool JumpTile
    {
        get
        {
            return m_jumpTile;
        }
        set
        {
            m_jumpTile = value;
        }
    }
}

public class Obstacle
{
    private GameObject m_gameObject;
    private bool[] m_freeLanes = new bool[3];

    public Obstacle(GameObject _go)
    {
        m_gameObject = _go;
    }
}