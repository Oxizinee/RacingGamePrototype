using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    public Text TimerText;
    public bool IsDriving = true;
    public Transform StartPos;
    public float timer;
    public bool restart;
    private float _resetTimer;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire4"))
        {
            restart = true;
        }
        if (Input.GetButton("Fire4"))
        {
            if (restart)
            {
                _resetTimer -= Time.deltaTime * 2f;
                if (_resetTimer < 0)
                {
                    gameObject.GetComponent<CarControll>().Start = true;
                    gameObject.GetComponent<CarControll>().Checkpoint1 = false;
                    gameObject.GetComponent<CarControll>().Checkpoint2 = false;
                    transform.position = StartPos.position;
                    transform.rotation = StartPos.rotation;

                    _resetTimer = 2;
                    timer = 0;
                    restart = false;
                }
            }
        }
        else
        {
            restart = false;
            _resetTimer = 2;
        }

        TimerMenager(IsDriving);
    }

    public void TimerMenager(bool start)
    {
        if (start)
        {
            TimerRunning();
        }
        else
        {
            StopTimer();
        }
    }
    public void StopTimer()
    {
        float tt = timer;
        TimerText.text = tt.ToString(); 
    }
    public void TimerRunning()
    {
        timer += Time.deltaTime;
        TimerText.text = timer.ToString();
    }
}
