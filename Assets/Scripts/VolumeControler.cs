using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControler : MonoBehaviour
{
    public Slider Slider;

    private void Start()
    {
        Slider = GetComponent<Slider>();

        float volume = PlayerPrefs.GetFloat("Volume", 1.0f);

        Slider.value = volume;
    }

}
