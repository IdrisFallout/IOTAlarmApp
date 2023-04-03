using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro; // Set this variable in the Unity Editor by dragging and dropping the TextMeshProUGUI GameObject onto it.

    void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm:ss");
    }
}

