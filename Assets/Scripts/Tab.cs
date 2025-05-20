using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Tab : MonoBehaviour
{
    public GameObject Panel;
    private Button Button;

    public static Tab Active;

    // Start is called before the first frame update
    void Start()
    {
        Button = GetComponent<Button>();

        if (Panel.activeSelf)
        {
            Button.interactable = false;
            Active = this;
        }
    }

    public void Press()
    {
        Panel.SetActive(true);
        Button.interactable = false;
        Active.Disable();
        Active = this;
    }

    public void Disable()
    {
        Panel.SetActive(false);
        Button.interactable = true;
    }
}
