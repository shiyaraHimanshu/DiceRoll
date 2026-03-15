using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectionHandler : MonoBehaviour
{
    public BGCOLOR currentSelectedColor;
    public CanvasGroup colorSelctionUIGroup;

    public void OnEnable()
    {
        if (colorSelctionUIGroup != null)
        {
            colorSelctionUIGroup.interactable = true;
            colorSelctionUIGroup.alpha = 1f;
        }
        if (DiceColor.Instance != null)
        {
            currentSelectedColor = DiceColor.Instance.currentSelctedColor;
        }
    }

    public void OnHideBar()
    {
        // Panel remains constantly on within the SettingPanel now
    }
    public void OnColorSelcted(int color)
    {
        switch (color)
        {
            case 0:
                SetColor(BGCOLOR.RANDOM);
                break;
            case 1:
                SetColor(BGCOLOR.GREEN);
                break;
            case 2:
                SetColor(BGCOLOR.ORANGE);
                break;
            case 3:
                SetColor(BGCOLOR.PINK);
                break;
            case 4:
                SetColor(BGCOLOR.BLUE);
                break;
            case 5:
                SetColor(BGCOLOR.PURPAL);
                break;
            case 6:
                SetColor(BGCOLOR.YELLOW);
                break;
            default:
                break;

        }
    }

    public void SetColor(BGCOLOR color)
    {
        currentSelectedColor = color;
        DiceColor.Instance.currentSelctedColor = color;
        
        PlayerPrefs.SetInt("DiceColor", (int)color);
        PlayerPrefs.Save();

        if (FirebaseAnalyticsManager.instance != null)
            FirebaseAnalyticsManager.instance.LogSettingChanged("dice_color", color.ToString());
    }
}
