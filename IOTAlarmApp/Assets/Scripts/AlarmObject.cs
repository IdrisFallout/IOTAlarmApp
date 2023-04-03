using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class AlarmObject : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite switchOn;

    [SerializeField]
    private Sprite switchOff;
    
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    private bool isSwitchedOn = true;

    private void Start()
    {
        // Assuming the Image component is attached to the same GameObject as this script
        image = GetComponent<Image>();
    }

    public void ToggleImageSprite()
    {
        // Toggle the current sprite
        if (isSwitchedOn)
        {
            image.sprite = switchOff;
            textMeshPro.text = "Alarm is off";
        }
        else
        {
            image.sprite = switchOn;
            textMeshPro.text = "Alarm is on";
        }

        // Toggle the boolean flag
        isSwitchedOn = !isSwitchedOn;
        
    }
}
