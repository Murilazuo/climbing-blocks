using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float timeToFill;
    void SetFill(int current, int max)
    {
        LeanTween.value(gameObject,SetFill, fill.fillAmount,(float)((float)current/(float)max) , timeToFill);
    }
    void SetFill(float fillAmount)
    {
        fill.fillAmount = fillAmount;
    }
    private void OnEnable()
    {
        DangerArea.OnSetDangerArea += SetFill;
    }
    private void OnDisable()
    {
        DangerArea.OnSetDangerArea -= SetFill;
    }
}
