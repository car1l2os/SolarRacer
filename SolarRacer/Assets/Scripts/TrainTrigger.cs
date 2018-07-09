using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrigger : MonoBehaviour
{

    Animator[] _animators;

    float _counter = 3f;
    bool _startCounter = false;
    bool _doneFlag = false;
    bool _jumped = false;

    private void Start()
    {
        _animators = transform.parent.GetComponentsInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (_startCounter)
        {
            _counter -= Time.deltaTime;
        }

        if (_counter <= 0.0f && _doneFlag == false)
        {
            if (_animators[0].gameObject.activeInHierarchy)
            {
                _animators[1].SetInteger("State", 2);
                _animators[0].SetInteger("State", 2);
                _doneFlag = true;
                _startCounter = false;
                _counter = 3f;
                _jumped = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<AudioManager>().Play("Train");
            foreach (Animator anim in _animators)
                anim.SetInteger("State", 1);
        }
        _doneFlag = false;
        _startCounter = true;
    }


    public bool Jumped
    {
        set
        {
            _jumped = value;
        }
    }

}
