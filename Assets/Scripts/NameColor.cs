using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class NameColor : MonoBehaviour
{
    public TMP_Text TMP_Text;
    public Image Image;

    public void SetName(string name)
    {
        ColorSet colorSet = ColorSet.GetColorSet(name);
        Image.color = colorSet.BackgroundColor;
        TMP_Text.color = colorSet.TextColor;
    }
}
