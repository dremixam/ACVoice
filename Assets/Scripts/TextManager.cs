using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public TextBoxEnabler TextBoxEnabler;
    public TMP_InputField TMP_InputField;
    public TMP_InputField TMP_NameField;

    public void PlayText()
    {
        TextBoxEnabler.PlayText(TMP_NameField.text, TMP_InputField.text);
    }
}
