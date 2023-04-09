using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
}
