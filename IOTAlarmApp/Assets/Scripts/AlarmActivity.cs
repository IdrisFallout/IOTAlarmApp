using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

    private void SendData(string url, List<Dictionary<string, object>> dataList)
    {
        // Serialize the list of dictionaries to JSON string
        string json = JsonConvert.SerializeObject(dataList);

        // Convert the JSON string to byte array
        byte[] postData = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, "POST"))
        {
            // Set the request body with the JSON data
            webRequest.uploadHandler = new UploadHandlerRaw(postData);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Send the web request and wait for response
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {
                // Wait for the request to complete
                Thread.Sleep(16); // Sleep for 16ms (approx. 1 frame at 60fps)
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Read the response content as text
                string responseBody = webRequest.downloadHandler.text;
                Debug.Log("Response: " + responseBody);
            }
            else
            {
                Debug.Log("Error: " + webRequest.error);
            }
            webRequest.Dispose();
        }
    }
    public void SendToCloud()
    {
        // Debug.Log(PackageAlarmJson()[0]);
        // PrintJsonObjectsList(PackageAlarmJson());
        // string url = "http://localhost:5000/endpoint";
        // StartCoroutine(GetResponse(url, PackageAlarmJson()));
        // StartCoroutine(SendData("http://localhost:5000/endpoint", PackageAlarmJson()));
        StartSendingData("http://localhost:5000/endpoint", PackageAlarmJson());
        // StartCoroutine(SendData("http://localhost:5000/endpoint", PackageAlarmJson()));
    }
    
    public void StartSendingData(string url, List<Dictionary<string, object>> dataList)
    {
        // Start a new thread for sending data
        Thread sendDataThread = new Thread(() => SendData(url, dataList));
        sendDataThread.Start();
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
