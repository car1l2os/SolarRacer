using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEventOnDisable : MonoBehaviour {

    private ScoreManager _scoreManager;
    [SerializeField]
    [Range(1, 2)]
    private int player;
    [SerializeField]
    private bool road;

    private void Start()
    {
        _scoreManager = transform.parent.GetComponent<ScoreManager>();
    }

    void OnDisable()
    {
        _scoreManager.AddToFreeScoreEvent(this.gameObject, player, road);
    }
}
