using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject[] tutorials;
    private void Start()
    {
        SetPlayer(SpawnPlayers.Instance.playerId);
    }
    void SetPlayer(int id)
    {
        tutorials[0].SetActive(false);
        tutorials[1].SetActive(false);
        tutorials[id-1].SetActive(true);
    }

    void OnStartConter()
    {
        panel.SetActive(false);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += OnStartConter;
        SpawnPlayers.OnSpawnPlayer += SetPlayer;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= OnStartConter;
        SpawnPlayers.OnSpawnPlayer -= SetPlayer;
        
    }
}
