using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour {

	public GameObject[] objectList;

    public GameObject[] objectSpawners;

    private int _currentSpawnTimer = 0;
    public int spawnDelay = 100;

	// Update is called once per frame
	void Update () {
        _currentSpawnTimer++;
        if (_currentSpawnTimer < spawnDelay)
        {
            return;
        } else
        {
		    for (int i = 0; i < objectSpawners.Length; i++)
            {
                int rndObj = Random.Range(0, objectList.Length);
                int rndObjSpawner = Random.Range(0, objectList.Length);
                Instantiate(objectList[rndObj], objectSpawners[rndObjSpawner].transform.position, Quaternion.identity);
            }
        }

        _currentSpawnTimer = 0;
	}
}
