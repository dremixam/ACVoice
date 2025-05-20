using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormField : MonoBehaviour
{
    public TMP_Text Label;
    public TMP_InputField Field;
    public string FieldName;

    private void OnValidate()
    {
        Label.text = FieldName;
    }

    public void Start()
    {

        Field.text = PlayerPrefs.GetString(FieldName);

    }

    public void SaveValue()
    {
        PlayerPrefs.SetString(FieldName, Field.text);
    }
}
