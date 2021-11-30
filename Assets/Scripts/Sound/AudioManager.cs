using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPref = "BackgroundPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";
    private int firstPlayInt;
    public Slider backgroundSlider, soundEffectsSlider;
    [SerializeField]
    private float backgroundFloat, soundEffectsFloat;
    public AudioSource backgroundAudio;
    public AudioSource[] soundEffectsAudio;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("The GameState is now ");

        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if (firstPlayInt == 0)
        {
            // game is run for the first time
            backgroundSlider.value = backgroundFloat;
            soundEffectsSlider.value = soundEffectsFloat;
            Debug.Log("Vorher" + PlayerPrefs.HasKey(BackgroundPref));
            Debug.Log("Vorher" + PlayerPrefs.HasKey(SoundEffectsPref));
            PlayerPrefs.SetFloat(BackgroundPref, backgroundFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsFloat);
            Debug.Log("Danach" + PlayerPrefs.HasKey(BackgroundPref));
            Debug.Log("Danach" + PlayerPrefs.HasKey(SoundEffectsPref));
            Debug.Log("value" + PlayerPrefs.GetFloat(BackgroundPref));
            Debug.Log("value" + PlayerPrefs.GetFloat(SoundEffectsPref));
            PlayerPrefs.SetInt(FirstPlay, -1);
            PlayerPrefs.Save();
        }
        else
        {
            // game was run already -> load sound configuration
            backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
            backgroundSlider.value = backgroundFloat;
            soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            soundEffectsSlider.value = soundEffectsFloat;
            Debug.Log("value" + PlayerPrefs.GetFloat(BackgroundPref));
            Debug.Log("value" + PlayerPrefs.GetFloat(SoundEffectsPref));

        }
    }

    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(BackgroundPref, backgroundSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsSlider.value);
        PlayerPrefs.Save();
    }

    private void OnApplicationFocus(bool inFocus)
    {
        if (!inFocus)
        {
            SaveSoundSettings();
        }
    }

    public void UpdateSound()
    {
        backgroundAudio.volume = backgroundSlider.value;
        for (int i=0; i < soundEffectsAudio.Length; i++)
        {
            soundEffectsAudio[i].volume = soundEffectsSlider.value;
        }
        SaveSoundSettings();
    }
}
