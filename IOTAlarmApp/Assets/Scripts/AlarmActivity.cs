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

    IEnumerator GetResponse(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

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
        }
    }

    public void SendToCloud()
    {
        // Debug.Log(PackageAlarmJson()[0]);
        PrintJsonObjectsList(PackageAlarmJson());
        // string url = "https://jsonplaceholder.typicode.com/todos/1";
        // StartCoroutine(GetResponse(url));
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
