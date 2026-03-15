using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCountSlider : MonoBehaviour
{
    public int minRange;
    public int maxRange;

    public int currentRange;

    public Slider slider;
    public Text rangeText;
    public CanvasGroup rangeUIGroup;

    public void OnEnable()
    {
        if (rangeUIGroup != null)
        {
            rangeUIGroup.interactable = true;
            rangeUIGroup.alpha = 1f;
        }
        if (DiceManager.instance != null && DiceManager.instance.rollController != null)
        {
            currentRange = DiceManager.instance.rollController.spawnedDice.Count;
            if (slider != null)
            {
                slider.SetValueWithoutNotify(currentRange);
            }
            if (rangeText != null)
            {
                rangeText.text = currentRange.ToString();
            }
        }
    }

    public void OnHideBar()
    {
        // Panel remains constantly on within the SettingPanel now
    }


    private void Start()
    {
        if (slider != null)
        {
            slider.minValue = minRange;
            slider.maxValue = maxRange;
            slider.wholeNumbers = true;
            slider.value = currentRange;

            // Add listener
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            // Initial update
            UpdateUI(slider.value);
        }
    }

    public void OnSliderValueChanged(float value)
    {
        UpdateUI(value);
    }

    private void UpdateUI(float value)
    {
        currentRange = Mathf.RoundToInt(value);
        if (rangeText != null)
        {
            rangeText.text = currentRange.ToString();
        }

        DiceManager.instance.rollController.SpawnAmount(currentRange);
        
        PlayerPrefs.SetInt("DiceCount", currentRange);
        PlayerPrefs.Save();
    }
}
