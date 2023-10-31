using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStartMatch : MonoBehaviour
{
    private void OnEnable()
    {
        MatchManager.OnStarGame += OnStartMatch;
    }
    private void OnDisable()
    {
        MatchManager.OnStarGame -= OnStartMatch;
    }

    void OnStartMatch()
    {
        gameObject.SetActive(false);
    }
}
