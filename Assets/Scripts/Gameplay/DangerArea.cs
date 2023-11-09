using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerArea : MonoBehaviour
{
    [SerializeField] float timeToStart;
    [SerializeField] float timeToMoveUp;
    [SerializeField] float moveUpDelay;
    [SerializeField] int maxFloor;
    IEnumerator StartBehaviour()
    {
        yield return new WaitForSeconds(timeToStart);
        for(float i = 0; i < maxFloor; i++)
        {
            LeanTween.scaleY(gameObject, i - .1f,timeToMoveUp);
            yield return new WaitForSeconds(moveUpDelay);
        }
    }

    void StartMatch()
    {
        StartCoroutine(nameof(StartBehaviour));
    }

    private void OnEnable()
    {
        MatchManager.OnStarGame += StartMatch;
    }
    private void OnDisable()
    {
        MatchManager.OnStarGame -= StartMatch;
    }
}

