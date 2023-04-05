using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Networking;

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
        
        StartCoroutine(GetRequest("https://jsonplaceholder.typicode.com/todos/1", HandleResponse));
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
    
    
    IEnumerator GetRequest(string uri, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                callback(webRequest.downloadHandler.text);
            }
        }
    }
    
    void HandleResponse(string response)
    {
        Debug.Log("Response: " + response);

        // Store the response in a variable
        string myResponse = response;
    }
}