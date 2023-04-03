using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetupAlarmPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TimeText;
    
    [SerializeField]
    private TextMeshProUGUI AmPmText;
    
    [SerializeField]
    private GameObject AlarmPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        DateTime time = System.DateTime.Now;
        TimeText.text = time.ToString("hh:mm");
        AmPmText.text = time.ToString("tt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ExpandAlarmPanel()
    {
        // print panel height
        Debug.Log(AlarmPanel.GetComponent<RectTransform>().rect.height);
        // increasing panel height
        Debug.Log("Increasing panel height");
    }
}