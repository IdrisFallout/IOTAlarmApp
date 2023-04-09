using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AlarmActivity : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    
    public GameObject availableAlarms;
    
    List<MyAlarmObject> alarmList = new List<MyAlarmObject>();
    
    public GameObject addWhere;
    public GameObject addWhat;
    
    private string url = "https://iotalarmapp.onrender.com";

    private void Start()
    {
        LoadAlarmsFromCloud();
    }

    private void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm:ss");
    }
    
    public void LoadAlarmsFromCloud()
    {
        StartCoroutine(GetAlarmCoroutine(url + "/get_alarm"));
    }
    
    public void SendToCloud()
    {
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
            Debug.Log("Response from server: " + responseData.status + " " + responseData.message);
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
            Debug.LogError("Failed to get alarm data from server. Error: " + request.error);
        }
        else
        {
            string responseContent = request.downloadHandler.text;
            // Parse the JSON response
            alarmList = JsonConvert.DeserializeObject<List<MyAlarmObject>>(responseContent);
            // Access and print out the JSON data
            foreach (MyAlarmObject alarm in alarmList)
            {
                Debug.Log("Index: " + alarm.index + ", Time: " + alarm.time + ", State: " + alarm.state);
                GameObject alarmObject = Instantiate(addWhat, addWhere.transform);
                AlarmObject alarmObjectScript = alarmObject.GetComponent<AlarmObject>();
                string time = alarm.time.Substring(0, alarm.time.Length - 3);
                string amPm = alarm.time.Substring(alarm.time.Length - 2);
                
                alarmObjectScript.timeText.text = time;
                if (alarm.state == false)
                {
                    alarmObjectScript.AlarmToggle();
                }
                alarmObjectScript.amPmText.text = amPm;
            }
        }

        request.Dispose();
    }
}
