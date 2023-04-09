using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAlarm : MonoBehaviour
{
    public GameObject addWhere;
    public GameObject addWhat;
    [HideInInspector]
    public bool isExpanded = false;

    [HideInInspector] public GameObject setAlarmPanel;
    
    private AlarmActivity alarmAppPanel;
    
    private void Start()
    {
        alarmAppPanel = GameObject.FindGameObjectWithTag("alarm-app-panel").GetComponent<AlarmActivity>();
    }
    
    public void AddMyAlarm()
    {
        if(alarmAppPanel.isStartup) return;
        if (isExpanded) return;
        setAlarmPanel = Instantiate(addWhat, addWhere.transform);
    }
}
