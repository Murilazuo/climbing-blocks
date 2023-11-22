using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{

    void OnStartGame()
    {
        MatchManager.Instance.SelectPlayerType(PlayerType.None);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += OnStartGame;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= OnStartGame;
    }
}
