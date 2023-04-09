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

    private void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm:ss");
    }
    
    public void SendToCloud()
    {
        StartCoroutine(SendJsonCoroutine("http://localhost:5000/endpoint", GetAlarmJson()));
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
}
