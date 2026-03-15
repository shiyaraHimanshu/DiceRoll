using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public SettingPanel settingPanel;
    public Button settingButton;
    public bool isSettingOpen = false;
    public Image soundImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    public Image vibrateImage;
    public Sprite vibrateOnSprite;
    public Sprite vibrateOffSprite;

    public void Start()
    {
        isSettingOpen = false;
        
        if (settingPanel != null)
        {
            settingPanel.gameObject.SetActive(false);
        }

        if (DiceManager.instance != null && DiceManager.instance.rollController != null && DiceManager.instance.rollController.diceRoll != null)
        {
            bool savedMute = PlayerPrefs.GetInt("SoundMute", 0) == 1;
            DiceManager.instance.rollController.diceRoll.mute = savedMute;
            UpdateSoundButton(savedMute);
        }

        // Initialize Vibration Button
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        UpdateVibrationButton(isVibrationOn);
    }

    public void OnSoundButtonClick()
    {
        if (DiceManager.instance != null && DiceManager.instance.rollController != null && DiceManager.instance.rollController.diceRoll != null)
        {
            bool isMuted = !DiceManager.instance.rollController.diceRoll.mute;
            DiceManager.instance.rollController.diceRoll.mute = isMuted;
            PlayerPrefs.SetInt("SoundMute", isMuted ? 1 : 0);
            PlayerPrefs.Save();
            UpdateSoundButton(isMuted);

            if (FirebaseAnalyticsManager.instance != null)
                FirebaseAnalyticsManager.instance.LogSettingChanged("sound", isMuted ? "off" : "on");
        }
    }

    public void UpdateSoundButton(bool isMuted)
    {
        if (soundImage != null)
        {
            if (isMuted)
            {
                soundImage.sprite = soundOffSprite;
            }
            else
            {
                soundImage.sprite = soundOnSprite;
            }
        }
    }

    public void OnVibrationButtonClick()
    {
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateVibrationButton(isVibrationOn);

        if (FirebaseAnalyticsManager.instance != null)
            FirebaseAnalyticsManager.instance.LogSettingChanged("vibration", isVibrationOn ? "on" : "off");
    }

    public void UpdateVibrationButton(bool isVibrationOn)
    {
        if (vibrateImage != null)
        {
            if (isVibrationOn)
            {
                vibrateImage.sprite = vibrateOnSprite;
            }
            else
            {
                vibrateImage.sprite = vibrateOffSprite;
            }
        }
    }

    public RangeUiHandler RangeUiHandler;
    public void OnRangeButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenRangePanel();
        }
    }


    public void OnDiceCountClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenDiceCountPanel();
        }
    }


    public ColorSelectionHandler ColorSelectionHandler;

    public void OnColorButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenColorPanel();
        }
    }

    public DiceRollAnimationSelection AnimationSelectionHandler;

    public void OnAnimationSelectButtonClick()
    {
        if (DiceManager.instance != null)
        {
            DiceManager.instance.OpenAnimationPanel();
        }
    }

    public void OnSettingButtonClick()
    {
        if (settingButton != null && !settingButton.interactable) return;

        if(isSettingOpen)
        {
            isSettingOpen = false;
            
            if(DiceManager.instance != null)
            {
                DiceManager.instance.isSettingsOpen = false;
                DiceManager.instance.CloseAllPanelsExcept(null);
            }
            if (settingPanel != null) settingPanel.Close();
        }
        else
        {
            isSettingOpen = true;
            if(DiceManager.instance != null)
            {
                DiceManager.instance.isSettingsOpen = true;
            }

            if (settingPanel != null) settingPanel.Open();
        }
    }
}
