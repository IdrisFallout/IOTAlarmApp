using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;


public class SetupAlarm : MonoBehaviour
{
    [Header("Setup Widget")]
    [SerializeField]
    private TMP_InputField hourInput;
    
    [SerializeField]
    private TMP_InputField minuteInput;
    
    [HideInInspector]
    public AddAlarm addAlarm;
    
    [SerializeField]
    private TextMeshProUGUI errorText;
    
    [SerializeField]
    private TMP_Dropdown amPmDropdown;

    private DateTime time;
    
    [Header("Add Alarm to list")]
    private GameObject addWhere;
    public GameObject addWhat;
    
    [HideInInspector] public GameObject AlarmPanel;
    
    String finalHour = "12", finalMinute = "00", finalAmPm = "AM";

    private void Start()
    {
        addAlarm = GameObject.FindGameObjectWithTag("AddAlarm").GetComponent<AddAlarm>();
        addWhere = GameObject.FindGameObjectWithTag("Alarm-scroll-content");
        addAlarm.isExpanded = true;
        time = System.DateTime.Now;
        // change placeholder
        hourInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("hh");
        minuteInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("mm");

        if (time.ToString("tt") == "AM")
        {
            // select index 0 in dropdown
            amPmDropdown.value = 0;
        }else if (time.ToString("tt") == "PM")
        {
            // select index 1 in dropdown
            amPmDropdown.value = 1;
        }
    }

    public void Cancel()
    {
        Destroy(addAlarm.setAlarmPanel);
        addAlarm.isExpanded = false;
    }

    public void Ok()
    {
        String hour = "", minute = "";
        if (hourInput.text == "" && minuteInput.text == "")
        {
            hour = hourInput.placeholder.GetComponent<TextMeshProUGUI>().text.ToString();
            minute = minuteInput.placeholder.GetComponent<TextMeshProUGUI>().text.ToString();
        }
        else
        {
            String hourString = hourInput.text;
            String minuteString = minuteInput.text;

            int hourInt = 0, minuteInt = 0;

            if (!int.TryParse(hourString, out hourInt) || !int.TryParse(minuteString, out minuteInt))
            {
                errorText.text = "Invalid Time Input";
                return;
            }

            if (!IsValidTime(hourInt, minuteInt))
            {
                errorText.text = "Invalid Time Input";
                return;
            }
            errorText.text = "";
            hour = hourInt.ToString();
            minute = minuteInt.ToString();
        }
        CreateAlarm(hour, minute, amPmDropdown.options[amPmDropdown.value].text);
    }


    public bool IsValidTime(int hour, int minute)
    {
        if (hour < 1 || hour > 12 || minute < 0 || minute > 59)
        {
            return false;
        }
        return true;
    }
    
    public void CreateAlarm(String hour, String minute, String amPm)
    {
        finalHour = FormatTime(hour);
        finalMinute = FormatTime(minute);
        finalAmPm = amPm;
        Destroy(addAlarm.setAlarmPanel);
        addAlarm.isExpanded = false;
        AlarmPanel = Instantiate(addWhat, addWhere.transform);
    }
    
    public String FormatTime(String time)
    {
        if (time.Length == 1)
        {
            time = "0" + time;
        }
        return time;
    }

    public String[] GetTime()
    {
        String[] myTime = new String[3];
        myTime[0] = finalHour;
        myTime[1] = finalMinute;
        myTime[2] = finalAmPm;
        Debug.Log(myTime[0] + " " + myTime[1] + " " + myTime[2]);
        return myTime;
    }

    // [SerializeField]
    // private TextMeshProUGUI TimeText;
    //
    // [SerializeField]
    // private TextMeshProUGUI AmPmText;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     DateTime time = System.DateTime.Now;
    //     TimeText.text = time.ToString("hh:mm");
    //     AmPmText.text = time.ToString("tt");
    // }
}
