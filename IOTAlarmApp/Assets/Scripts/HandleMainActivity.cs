using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMainActivity : MonoBehaviour
{
    public GameObject panelPrefab;
    public GameObject scrollContent;

    public void AddAlarm()
    {
        GameObject newPanel = Instantiate(panelPrefab, scrollContent.transform);
        // You can set any additional properties or behaviors for the new panel here
    }
}
