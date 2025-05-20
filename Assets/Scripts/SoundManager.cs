using System.Collections;
using System.Collections.Generic;
using Developpez.Dotnet;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public List<AudioClip> audioClips;
    public float LetterDuration = 0.1f;
    private Dictionary<char, AudioClip> letterSounds;
    public List<CharacterDuration> PauseCharacters;

    private AudioSource AudioSource;



    void Awake()
    {
        Application.runInBackground = true;
    }

    public void Start()
    {
        letterSounds = new Dictionary<char, AudioClip>();
        AudioSource = GetComponent<AudioSource>();

        float volume = PlayerPrefs.GetFloat("Volume", 1.0f);

        SetVolume(volume);

        foreach (AudioClip audioClip in audioClips)
        {
            letterSounds.Add(audioClip.name[0], audioClip);
        }
    }

    public IEnumerator PlayLetter(char letter)
    {
        

        if (PauseCharacters.Exists(x => x.Character == letter))
        {
            CharacterDuration characterDuration = PauseCharacters.Find(x => x.Character == letter);
            yield return new WaitForSeconds(characterDuration.Duration);
        }
        else if (letterSounds.ContainsKey(letter))
        {
            AudioSource.PlayOneShot(letterSounds[letter]);

            yield return new WaitForSeconds(LetterDuration);
        }
        else
        {
            yield return null;
        }
    }

    public void SetVolume(float volume)
    {
        float vol = Mathf.Log10(volume) * 20;

        if (volume == 0)
        {
            vol = -80.0f;
        }

        audioMixer.SetFloat("Volume", vol);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    [System.Serializable]
    public struct CharacterDuration
    {
        public char Character;
        public float Duration;
    }
}
