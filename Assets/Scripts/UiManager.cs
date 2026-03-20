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
    public GameObject soundOnObject;
    public GameObject soundOffObject;

    public GameObject vibrateOnObject;
    public GameObject vibrateOffObject;

    public Image selectedAnimationbase;
    public Image selectedColorbase;

    public List<Button> currentColors;
    public List<GameObject> currentAnimations;

    public static UiManager instance;

    public void Awake()
    {
        instance = this;
    }

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

        // Initialize Selection Positions with a slight delay to ensure layout is ready
        StartCoroutine(SetInitialSelectionPositions());
    }

    private IEnumerator SetInitialSelectionPositions()
    {
        // Wait until end of frame so layout groups and positions are calculated
        yield return new WaitForEndOfFrame();
        
        BGCOLOR savedColor = (BGCOLOR)PlayerPrefs.GetInt("DiceColor", 0);
        UpdateSelectedColorPosition(savedColor);
        UpdateSelectedAnimationPosition(PlayerPrefs.GetInt("DiceAnimation", 0));
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
        if (soundOnObject != null)
        {
            soundOnObject.SetActive(!isMuted);
        }
        if (soundOffObject != null)
        {
            soundOffObject.SetActive(isMuted);
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
        if (vibrateOnObject != null)
        {
            vibrateOnObject.SetActive(isVibrationOn);
        }
        if (vibrateOffObject != null)
        {
            vibrateOffObject.SetActive(!isVibrationOn);
        }
    }

    public void SetSelectionIndicatorsActive(bool isActive)
    {
        if (selectedColorbase != null) selectedColorbase.gameObject.SetActive(isActive);
        if (selectedAnimationbase != null) selectedAnimationbase.gameObject.SetActive(isActive);
    }

    public void UpdateSelectedColorPosition(BGCOLOR color)
    {
        int index = GetColorUIIndex(color);
        if (selectedColorbase != null && currentColors != null && index >= 0 && index < currentColors.Count)
        {
            selectedColorbase.gameObject.SetActive(true);
            selectedColorbase.transform.position = currentColors[index].transform.position;
        }
    }

    public void UpdateSelectedAnimationPosition(int index)
    {
        if (selectedAnimationbase != null && currentAnimations != null && index >= 0 && index < currentAnimations.Count)
        {
            selectedAnimationbase.gameObject.SetActive(true);
            selectedAnimationbase.transform.position = currentAnimations[index].transform.position;
        }
    }

    private int GetColorUIIndex(BGCOLOR color)
    {
        switch (color)
        {
            case BGCOLOR.RANDOM: return 0;
            case BGCOLOR.GREEN: return 1;
            case BGCOLOR.ORANGE: return 2;
            case BGCOLOR.PINK: return 3;
            case BGCOLOR.BLUE: return 4;
            case BGCOLOR.PURPAL: return 5;
            case BGCOLOR.YELLOW: return 6;
            default: return 0;
        }
    }

    public void RefreshSettingsUI()
    {
        // Refresh Sound
        bool isMuted = PlayerPrefs.GetInt("SoundMute", 0) == 1;
        UpdateSoundButton(isMuted);

        // Refresh Vibration
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        UpdateVibrationButton(isVibrationOn);

        // Hide indicators during expansion to avoid visual jumps
        SetSelectionIndicatorsActive(false);
    }

    public void FinalizeSelectionPositions()
    {
        StartCoroutine(SetInitialSelectionPositions());
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

    public void OnRemoveAdsButtonClick()
    {
        if (IAPManager.instance != null)
        {
            IAPManager.instance.BuyNoAds();
        }
        else
        {
            Debug.LogError("IAPManager instance is null.");
        }
    }
}
