using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDataContainer : MonoBehaviour
{

    public enum Difficulty { Easy, Medium, Hard };

    public static int _player1Points = 0;
    public static int _player2Points = 0;

    public static int _player1Collectables = 0;
    public static int _player2Collectables = 0;

    public static int _player1PickUps = 0;
    public static int _player2PickUps = 0;

    public static float _player1SecondsInShadow = 0.0f;
    public static float _player2SecondsInShadow = 0.0f;

    public static int _pointsPerCollectableGroup = 0;
    public static int _pointsPerPickUp = 0;

    public static System.Boolean _controlledByIA = false;
    public static Difficulty difficulty = Difficulty.Easy;

    public static int _firstInFinish = 0;


    public static void ResetStats()
    {
        _player1Points = 0;
        _player2Points = 0;

        _player1Collectables = 0;
        _player2Collectables = 0;

        _player1PickUps = 0;
        _player2PickUps = 0;

        _player1SecondsInShadow = 0f;
        _player2SecondsInShadow = 0f;

        _firstInFinish = 0;
        _controlledByIA = false;
        difficulty = Difficulty.Easy;
    }
}
