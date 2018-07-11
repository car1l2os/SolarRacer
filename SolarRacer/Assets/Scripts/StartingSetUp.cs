using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSetUp : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        StaticDataContainer._player1Points = 0;
        StaticDataContainer._player2Points = 0;

        StaticDataContainer._player1Collectables = 0;
        StaticDataContainer._player2Collectables = 0;

        StaticDataContainer._player1PickUps = 0;
        StaticDataContainer._player2PickUps = 0;

        StaticDataContainer._player1SecondsInShadow = 0.0f;
        StaticDataContainer._player2SecondsInShadow = 0.0f;

        StaticDataContainer._pointsPerCollectableGroup = 0;
        StaticDataContainer._pointsPerPickUp = 0;

        StaticDataContainer._controlledByIA = false;

        StaticDataContainer._firstInFinish = 0;
    }
}
