using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private const string FirstPlay = "FirstPlay";
    private const string BackgroundPref = "BackgroundPref";
    private const string SoundEffectsPref = "SoundFXPref";
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

        // TODO loading of slider values does not work -> assume game is started first time to get default values
        // firstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        firstPlayInt = 0;

        if (firstPlayInt == 0)
        {
            // game is run for the first time
            backgroundSlider.value = backgroundFloat;
            soundEffectsSlider.value = soundEffectsFloat;
            PlayerPrefs.SetFloat(BackgroundPref, backgroundFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
            PlayerPrefs.Save();
        }
        else
        {
            // game was run already -> load sound configuration
            backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
            backgroundSlider.value = backgroundFloat;
            soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            soundEffectsFloat = PlayerPrefs.GetFloat("Tests");
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
