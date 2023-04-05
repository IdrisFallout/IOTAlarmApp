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
    
    public AddAlarm addAlarm;

    private void Start()
    {
        addAlarm = GameObject.FindGameObjectWithTag("AddAlarm").GetComponent<AddAlarm>();
        addAlarm.isExpanded = true;
        // change placeholder
        DateTime time = System.DateTime.Now;
        hourInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("hh");
        minuteInput.placeholder.GetComponent<TextMeshProUGUI>().text = time.ToString("mm");
    }

    private void Cancel()
    {
        
    }

    private void Ok()
    {
        
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
