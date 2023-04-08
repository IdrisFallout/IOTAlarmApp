using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AlarmActivity : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    
    private const string endpointUrl = "http://localhost:5000/endpoint";

    private void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm:ss");
    }

    private IEnumerator SendNetworkRequest()
    {
        // Prepare data as a dictionary
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "name", "John" },
            { "age", 30 },
            { "city", "New York" }
        };

        // Convert dictionary to JSON string
        string json = JsonUtility.ToJson(data);
        byte[] postData = Encoding.UTF8.GetBytes(json);

        // Create UnityWebRequest with POST method
        UnityWebRequest request = UnityWebRequest.Post(endpointUrl, "");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request asynchronously
        yield return request.SendWebRequest();

        // Check for errors
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }


    public void SendToCloud()
    {
        // Debug.Log(PackageAlarmJson()[0]);
        // PrintJsonObjectsList(PackageAlarmJson());
        // string url = "http://localhost:5000/endpoint";
        // StartCoroutine(GetResponse(url, PackageAlarmJson()));
        StartCoroutine(SendNetworkRequest());
    }
    public class MyAlarm
    {
        public int index { get; set; }
        public string time { get; set; }
        public bool state { get; set; }
    }
    
    public static List<Dictionary<string, object>> PackageAlarmJson()
    {
        List<Dictionary<string, object>> jsonAlarmObjectsList = new List<Dictionary<string, object>>();
        
        MyAlarm myAlarm = new MyAlarm
        {
            index = 0,
            time = "12:00 AM",
            state = true
        };
        
        MyAlarm myAlarm1 = new MyAlarm
        {
            index = 1,
            time = "08:00 PM",
            state = false
        };
        
        Dictionary<string, object> jsonAlarmObject = new Dictionary<string, object>
        {
            { "index", myAlarm.index },
            { "time", myAlarm.time },
            { "state", myAlarm.state }
        };
        
        Dictionary<string, object> jsonAlarmObject1 = new Dictionary<string, object>
        {
            { "index", myAlarm1.index },
            { "time", myAlarm1.time },
            { "state", myAlarm1.state }
        };
        
        jsonAlarmObjectsList.Add(jsonAlarmObject);
        jsonAlarmObjectsList.Add(jsonAlarmObject1);
        
        return jsonAlarmObjectsList;
    }
    
    public static void PrintJsonObjectsList(List<Dictionary<string, object>> jsonObjectsList)
    {
        foreach (var jsonObject in jsonObjectsList)
        {
            Debug.Log("{ index : " + jsonObject["index"] + ", time : " + jsonObject["time"] + ", state : " + jsonObject["state"] + " }");
        }
    }
    
}
