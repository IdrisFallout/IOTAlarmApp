using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAlarm : MonoBehaviour
{
    public GameObject addWhere;
    public GameObject addWhat;
    public bool isExpanded = false;
    public void AddMyAlarm()
    {
        if (isExpanded) return;
        GameObject newPanel = Instantiate(addWhat, addWhere.transform);
        isExpanded = true;
    }
}
