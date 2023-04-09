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
    
    public string GetAlarmJson()
    {
        List<MyAlarmObject> myObjects = new List<MyAlarmObject>
        {
            new MyAlarmObject { index = 0, time = "12:30 PM", state = true },
            new MyAlarmObject { index = 1, time = "3:45 PM", state = false },
            new MyAlarmObject { index = 2, time = "6:15 AM", state = true }
        };

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
            Debug.Log("Response from server: " + responseContent);
        }
        
        request.Dispose();
    }
}
