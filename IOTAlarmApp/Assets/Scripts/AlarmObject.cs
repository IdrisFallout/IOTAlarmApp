using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Threading;

public class AlarmObject : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite switchOn;

    [SerializeField]
    private Sprite switchOff;
    
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    [HideInInspector]
    public bool isSwitchedOn = true;
    
    [SerializeField]
    public TextMeshProUGUI timeText;
    
    [SerializeField]
    public TextMeshProUGUI amPmText;

    private AlarmActivity alarmAppPanel;
    
    [SerializeField]
    private GameObject timeRemaining;

    private void Start()
    {
        alarmAppPanel = GameObject.FindGameObjectWithTag("alarm-app-panel").GetComponent<AlarmActivity>();
    }

    public void Initialize()
    {
        Start();
    }

    public void AlarmToggle()
    {
        // Toggle the current sprite
        if (isSwitchedOn)
        {
            image.sprite = switchOff;
            textMeshPro.text = "Alarm is off";
        }
        else
        {
            image.sprite = switchOn;
            textMeshPro.text = "Alarm is on";
            // Call the function TimeRemaining in a coroutine
            Thread thread = new Thread(TimeRemaining);
            thread.Start();
        }

        // Toggle the boolean flag
        isSwitchedOn = !isSwitchedOn;
        
        if(alarmAppPanel.isStartup) return;
        alarmAppPanel.isSynced = false;
        alarmAppPanel.CheckSync();
    }
    
    public void DeleteAlarm()
    {
        Destroy(gameObject);
        if(alarmAppPanel.isStartup) return;
        alarmAppPanel.isSynced = false;
        alarmAppPanel.CheckSync();
    }

    public void TimeRemaining()
    {
        Instantiate(timeRemaining, alarmAppPanel.transform);
        Thread.Sleep(1000);
        Destroy(timeRemaining);
    }
}
