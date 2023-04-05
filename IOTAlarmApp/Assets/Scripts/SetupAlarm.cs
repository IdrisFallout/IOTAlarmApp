using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;


public class SetupAlarm : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField hourInput;
    
    [SerializeField]
    private TMP_InputField minuteInput;
    
    [HideInInspector]
    public AddAlarm addAlarm;
    
    [SerializeField]
    private TextMeshProUGUI errorText;

    private void Start()
    {
        addAlarm = GameObject.FindGameObjectWithTag("AddAlarm").GetComponent<AddAlarm>();
        addAlarm.isExpanded = true;
        // change placeholder
        DateTime time = System.DateTime.Now;
        hourInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("hh");
        minuteInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("mm");
    }

    public void Cancel()
    {
        Destroy(addAlarm.setAlarmPanel);
        addAlarm.isExpanded = false;
    }

    public void Ok()
    {
        String hour = "", minute = "";
        if (hourInput.text == "" || minuteInput.text == "")
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
        Debug.Log(hour);
        Debug.Log(minute);
        Destroy(addAlarm.setAlarmPanel);
        addAlarm.isExpanded = false;
    }


    public bool IsValidTime(int hour, int minute)
    {
        if (hour < 1 || hour > 12 || minute < 0 || minute > 59)
        {
            return false;
        }
        return true;
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
