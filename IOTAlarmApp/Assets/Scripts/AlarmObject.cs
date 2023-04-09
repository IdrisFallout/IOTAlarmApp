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

    private SetupAlarm setupAlarm;

    private void Start()
    {
        // Assuming the Image component is attached to the same GameObject as this script
        setupAlarm = GameObject.FindGameObjectWithTag("setup-alarm").GetComponent<SetupAlarm>();
        String[] time = setupAlarm.GetTime();
        timeText.text = time[0] + ":" + time[1];
        amPmText.text = time[2];
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
        
    }
    
    public void DeleteAlarm()
    {
        Destroy(gameObject);
    }
}
