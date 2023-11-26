using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSteps : MonoBehaviour
{
    [SerializeField] Image[] stepIcons;
    [SerializeField] GameObject[] steps;
    int currentStep;
    private void Start()
    {
        Set(0);
    }
    public void Next()
    {
        Set(currentStep+1);
    }
    public void Previusly()
    {
        Set(currentStep-1);
    }
    void Set(int i)
    {
        steps[currentStep].SetActive(false);
        LeanTween.scale(stepIcons[currentStep].gameObject, Vector3.one, .3f);
        stepIcons[currentStep].color = new(152f / 255f, 152f/255f, 152f / 255f);

        if(i < 0)
            i = steps.Length-1;
        else if (i >= steps.Length)
            i = 0;

        currentStep = i;

        steps[currentStep].SetActive(true);
        stepIcons[currentStep].color = Color.white;
        LeanTween.scale(stepIcons[currentStep].gameObject, Vector3.one * 2f, .3f);

    }
}
