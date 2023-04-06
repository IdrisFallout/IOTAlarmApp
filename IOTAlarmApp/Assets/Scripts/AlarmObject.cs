using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

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

    private bool isSwitchedOn = true;
    
    [SerializeField]
    private TextMeshProUGUI timeText;
    
    [SerializeField]
    private TextMeshProUGUI amPmText;

    private SetupAlarm setupAlarm;
    
    [HideInInspector] public GameObject addWhere;
    [Header("Modify Alarm")]
    public GameObject addWhat;
    [HideInInspector] public bool isExpanded = false;

    [HideInInspector] public GameObject setAlarmPanel;

    private void Start()
    {
        // Assuming the Image component is attached to the same GameObject as this script
        setupAlarm = GameObject.FindGameObjectWithTag("setup-alarm").GetComponent<SetupAlarm>();
        addWhere = GameObject.FindGameObjectWithTag("alarm-app-panel");
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
    
    public void AddMyAlarm()
    {
        if (isExpanded) return;
        setAlarmPanel = Instantiate(addWhat, addWhere.transform);
    }
}
