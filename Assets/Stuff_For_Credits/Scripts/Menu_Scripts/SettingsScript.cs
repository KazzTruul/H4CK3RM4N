using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/*By Björn Andersson*/

//Script to handle all game settings.

public class SettingsScript : MonoBehaviour
{
    [SerializeField]
    AudioMixer mixer;

    [SerializeField]
    Slider sfxSlider, masterVolumeSlider, musicSlider;

    private void Start()
    {
        sfxSlider.value = XMLManager.GetVolume("SFX");
        masterVolumeSlider.value = XMLManager.GetVolume("Master");
        musicSlider.value = XMLManager.GetVolume("Music");
        SetMusicVolume(XMLManager.GetVolume("Music"));
        SetMasterVolume(XMLManager.GetVolume("Master"));
        SetSFXVolume(XMLManager.GetVolume("SFX"));
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("Music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("Music", volume);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ApplySettings()
    {
        XMLManager.SaveAudioSetting("Master", masterVolumeSlider.value);
        XMLManager.SaveAudioSetting("Music", musicSlider.value);
        XMLManager.SaveAudioSetting("SFX", sfxSlider.value);
    }

    public void DiscardChanges()
    {
        sfxSlider.value = XMLManager.GetVolume("SFX");
        masterVolumeSlider.value = XMLManager.GetVolume("Master");
        musicSlider.value = XMLManager.GetVolume("Music");
    }
}