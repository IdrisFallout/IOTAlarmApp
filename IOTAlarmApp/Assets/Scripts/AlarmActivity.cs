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
        Debug.Log(PackageAlarmJson()["time"]);
        // string url = "https://jsonplaceholder.typicode.com/todos/1";
        // StartCoroutine(GetResponse(url));
    }
    public class MyAlarm
    {
        public int Number { get; set; }
        public string Time { get; set; }
        public bool State { get; set; }
    }
    
    public static Dictionary<string, object> PackageAlarmJson()
    {
        MyAlarm myAlarm = new MyAlarm
        {
            Number = 1,
            Time = "12:00 AM",
            State = true
        };
        
        Dictionary<string, object> jsonAlarmObject = new Dictionary<string, object>
        {
            { "number", myAlarm.Number },
            { "time", myAlarm.Time },
            { "state", myAlarm.State }
        };

        return jsonAlarmObject;
    }
    
}
