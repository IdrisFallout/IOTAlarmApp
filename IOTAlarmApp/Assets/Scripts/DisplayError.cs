using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayError : MonoBehaviour
{
    public GameObject errorText;
    
    private AlarmActivity alarmAppPanel;
    
    private void Start()
    {
        alarmAppPanel = GameObject.FindGameObjectWithTag("alarm-app-panel").GetComponent<AlarmActivity>();
    }
    
    public void DisplayErrorText(string error)
    {
        errorText.GetComponent<TMPro.TextMeshProUGUI>().text = error;
    }

    public void Retry()
    {
        alarmAppPanel.LoadAlarmsFromCloud();
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}
