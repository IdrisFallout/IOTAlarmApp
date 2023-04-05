using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAlarm : MonoBehaviour
{
    public GameObject addWhere;
    public GameObject addWhat;
    public bool isExpanded = false;

    [HideInInspector] public GameObject setAlarmPanel;
    public void AddMyAlarm()
    {
        if (isExpanded) return;
        setAlarmPanel = Instantiate(addWhat, addWhere.transform);
    }
}
