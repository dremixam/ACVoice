using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Logger : MonoBehaviour
{
    private static Logger Instance;

    public TMP_Text TMP_Text;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Log(string str)
    {
        if (Instance != null && Instance.enabled)
        {
            Instance.AppendToLog(str);
        }
    }

    private void AppendToLog(string str)
    {
        TMP_Text.text += "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + str + "\n";
    }
}
