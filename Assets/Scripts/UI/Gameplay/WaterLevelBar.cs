using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float timeToFill;
    [SerializeField] CanvasGroup canvasGroup;
    private void Start()
    {
        canvasGroup.alpha = 0f;
    }
    void SetFill(int current, int max)
    {
        LeanTween.value(gameObject,SetFill, fill.fillAmount,(float)((float)current/(float)max) , timeToFill);
    }
    void SetFill(float fillAmount)
    {
        fill.fillAmount = fillAmount;
    }
    void ResetBar()
    {
        LeanTween.cancel(gameObject);
        fill.fillAmount = 0;
        canvasGroup.alpha = 0;
    }
    private void StartConter()
    {
        canvasGroup.alpha = 1;
    }
    private void OnEnable()
    {
        DangerArea.OnSetDangerArea += SetFill;
        MatchManager.OnPlayAgain += ResetBar;
        MatchManager.OnStarCounter += StartConter;
    }
    private void OnDisable()
    {
        DangerArea.OnSetDangerArea -= SetFill;
        MatchManager.OnPlayAgain -= ResetBar;
        MatchManager.OnStarCounter -= StartConter;
        
    }
}
