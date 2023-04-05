using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SetupAlarm : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField hourInput;
    
    [SerializeField]
    private TMP_InputField minuteInput;


    private void Start()
    {
        // change placeholder
        hourInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Hour";
        minuteInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Minute";
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
