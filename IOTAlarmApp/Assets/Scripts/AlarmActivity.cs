using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;

public class AlarmActivity : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    
    public GameObject availableAlarms;
    
    List<MyAlarmObject> alarmList = new List<MyAlarmObject>();
    
    public GameObject addWhere;
    public GameObject addWhat;
    // http://127.0.0.1:5000 or https://iotalarmapp.onrender.com
    private string url = "http://127.0.0.1:5000";
    
    [Header("Check if synced")]
    [HideInInspector]
    public bool isSynced = false;
    
    [SerializeField]
    private Image syncimage;

    [SerializeField]
    private Sprite synced;

    [SerializeField]
    private Sprite notSynced;
    
    [HideInInspector] public bool isStartup = true;
    
    [Header("Error Panel")]
    private GameObject errorPanel;
    public GameObject addWhereError;
    public GameObject addWhatError;
    

    private void Start()
    {
        LoadAlarmsFromCloud();
    }

    private void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm tt");
    }
    
    public void LoadAlarmsFromCloud()
    {
        isStartup = true;
        StartCoroutine(GetAlarmCoroutine(url + "/get_alarm"));
    }
    
    public void SendToCloud()
    {
        if(isStartup) return;
        StartCoroutine(SendJsonCoroutine(url + "/set_alarm", GetAlarmJson()));
    }

    public class MyAlarmObject
    {
        public int index { get; set; }
        public string time { get; set; }
        public bool state { get; set; }
    }
    
    public class ResponseJson
    {
        public string message { get; set; }
        public string status { get; set; }
    }
    
    public string GetAlarmJson()
    {
        List<MyAlarmObject> myObjects = new List<MyAlarmObject>();
        
        for (int i = 0; i < availableAlarms.transform.childCount; i++)
        {
            GameObject child = availableAlarms.transform.GetChild(i).gameObject;
            AlarmObject alarmObject = child.GetComponent<AlarmObject>();
            string time = alarmObject.timeText.text;
            bool state = alarmObject.isSwitchedOn;
            MyAlarmObject myAlarmObject = new MyAlarmObject();
            myAlarmObject.index = i;
            myAlarmObject.time = time + " " + alarmObject.amPmText.text;
            myAlarmObject.state = state;
            myObjects.Add(myAlarmObject);
        }
        string json = JsonConvert.SerializeObject(myObjects, Formatting.Indented);
        return json;
    }
    
    private IEnumerator SendJsonCoroutine(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string auth = "admin" + ":" + "admin";
        string authEncoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(auth));
        request.SetRequestHeader("Authorization", "Basic " + authEncoded);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Failed to send JSON to server. Error: " + request.error);
        }
        else
        {
            string responseContent = request.downloadHandler.text;
            ResponseJson responseData = JsonConvert.DeserializeObject<ResponseJson>(responseContent);
            string the_response = "Response from server: " + responseData.status + " " + responseData.message;
            Debug.Log(the_response);
            
            isSynced = true;
            CheckSync();
        }

        request.Dispose();
    }
    private IEnumerator GetAlarmCoroutine(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin:admin")));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            string the_response = "Failed to get alarm data from server. Error: " + request.error;
            Debug.LogError(the_response);
            errorPanel = Instantiate(addWhatError, addWhereError.transform);
            DisplayError displayError = errorPanel.GetComponent<DisplayError>();
            displayError.DisplayErrorText(the_response);
        }
        else
        {
            string responseContent = request.downloadHandler.text;
            // Parse the JSON response
            alarmList = JsonConvert.DeserializeObject<List<MyAlarmObject>>(responseContent);
            // Access and print out the JSON data
            foreach (MyAlarmObject alarm in alarmList)
            {
                // Debug.Log("Index: " + alarm.index + ", Time: " + alarm.time + ", State: " + alarm.state);
                GameObject alarmObject = Instantiate(addWhat, addWhere.transform);
                AlarmObject alarmObjectScript = alarmObject.GetComponent<AlarmObject>();
                string time = alarm.time.Substring(0, alarm.time.Length - 3);
                string amPm = alarm.time.Substring(alarm.time.Length - 2);
                
                alarmObjectScript.Initialize();
                
                alarmObjectScript.timeText.text = time;
                if (alarm.state == false)
                {
                    alarmObjectScript.AlarmToggle();
                }
                alarmObjectScript.amPmText.text = amPm;
            }
            isSynced = true;
            CheckSync();
            isStartup = false;
        }

        request.Dispose();
    }
    
    public void CheckSync()
    {
        syncimage.sprite = isSynced ? synced : notSynced;
    }
}
