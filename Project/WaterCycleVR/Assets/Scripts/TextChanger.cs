using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextChanger : MonoBehaviour
{

    public string text;
    public TextMeshProUGUI TextPro;

    public void ChangeText()
    {
        TextPro.text = text;
    }
}
