using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegressiveCounter : MonoBehaviour
{
    int count = 3;
    [SerializeField] TMP_Text text;
    [SerializeField] Animator anim;
    readonly int countTrigger = Animator.StringToHash("Count");
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    void StartCount()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        CountOne();
    }
    void CountOne()
    {
        anim.SetTrigger(countTrigger);
        
        if (count == 0)
            text.text = "GO!";
        else
            text.text = count.ToString();

        LeanTween.delayedCall(1, () => {
            if (count >= 0) CountOne();
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                MatchManager.Instance.StartMatch();
            }
        });

        count--;
    }
    
    private void OnEnable()
    {
        MatchManager.OnStarCounter += StartCount;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= StartCount;
    }
}
