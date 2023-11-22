using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject[] tutorials;
    
    void SetPlayer(PlayerType playerType)
    {
        tutorials[0].SetActive((int)playerType == 0);
        tutorials[1].SetActive((int)playerType == 1);
    }

    void OnStartConter()
    {
        panel.SetActive(false);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += OnStartConter;
        MatchManager.OnSelectPlayerType += SetPlayer;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= OnStartConter;
        MatchManager.OnSelectPlayerType -= SetPlayer;
        
    }
}
