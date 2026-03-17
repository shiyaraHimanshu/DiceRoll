using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DiceRollAnimationSelection : MonoBehaviour
{

    public Dice mainDice;
    public Dice CloneDice;
    public Dice simpleMainDice;

    public RectTransform mainDiceRect;
    public RectTransform CloneDiceRect;

    public CanvasGroup AnimationUISelectcanvasGroup;

    public void OnEnable()
    {
        if (AnimationUISelectcanvasGroup != null)
        {
            AnimationUISelectcanvasGroup.interactable = true;
            AnimationUISelectcanvasGroup.alpha = 1f;
        }
        StartCoroutine(PlayDiceRollAnimation());
        StartCoroutine(PlaySimpleRoll());
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnHideBar()
    {
        // Panel remains constantly on within the SettingPanel now
    }

    private IEnumerator PlayDiceRollAnimation()
    {
        while (true)
        {
            yield return StartCoroutine(PlaySlideAnimation());
        }
    }

    IEnumerator PlaySlideAnimation()
    {
        float i = 0;
        mainDiceRect.pivot = new Vector2(0, 0.5f);
        CloneDiceRect.pivot = new Vector2(1, 0.5f);
        CloneDiceRect.offsetMin = Vector2.zero;
        CloneDiceRect.offsetMax = Vector2.zero;
        mainDiceRect.transform.localScale = new Vector3(1, 1, 1);
        CloneDiceRect.transform.localScale = new Vector3(0, 1, 1);
        CloneDice.gameObject.SetActive(true);
        CloneDice.Roll();
        Vector3 MDiceCurrentScale = mainDiceRect.transform.localScale;
        Vector3 CDiecCurrentScale = CloneDiceRect.transform.localScale;
        while (i < 1)
        {
            i += Time.deltaTime * 3;
            mainDiceRect.transform.localScale = Vector3.Lerp(MDiceCurrentScale, CDiecCurrentScale, i);
            CloneDiceRect.transform.localScale = Vector3.Lerp(CDiecCurrentScale, MDiceCurrentScale, i);
            yield return null;
        }
        mainDice.gameObject.SetActive(false);
        mainDiceRect.pivot = new Vector2(1, 0.5f);
        mainDiceRect.offsetMin = Vector2.zero;
        mainDiceRect.offsetMax = Vector2.zero;
        CloneDiceRect.pivot = new Vector2(0, 0.5f);
        mainDice.gameObject.SetActive(true);
        i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 3;
            mainDiceRect.transform.localScale = Vector3.Lerp(CDiecCurrentScale, MDiceCurrentScale, i);
            CloneDiceRect.transform.localScale = Vector3.Lerp(MDiceCurrentScale, CDiecCurrentScale, i);
            yield return null;
        }
        CloneDice.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator PlaySimpleRoll()
    {
        while(true)
        {
            simpleMainDice.Roll();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void OnSelectAniamtion(int value)
    {
        switch (value)
        {
            case 0:
                DiceManager.instance.currentAnimation = ANIM.SLIDE;
                break;
            case 1:
                DiceManager.instance.currentAnimation = ANIM.SIMPLE;
                break;
            case 2:
               DiceManager.instance.currentAnimation = ANIM.NONE;
                break;
            default:
                break;
        }

        PlayerPrefs.SetInt("DiceAnimation", value);
        PlayerPrefs.Save();

        if (UiManager.instance != null)
        {
            UiManager.instance.UpdateSelectedAnimationPosition(value);
        }

        if (FirebaseAnalyticsManager.instance != null)
            FirebaseAnalyticsManager.instance.LogSettingChanged("dice_animation", DiceManager.instance.currentAnimation.ToString());

    }
}
