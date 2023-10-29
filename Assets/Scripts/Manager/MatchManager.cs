using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviourPunCallbacks
{
    static MatchManager instance;
    public static MatchManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<MatchManager>();
            return instance;
        }
    }

    public static System.Action OnStarGame;

    
    private void Awake()
    {
        instance = this;
    }
    public void PlayerPiecesWin()
    {
        NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_WIN_GAME_EVENT);
    }
    public void PlayerPlatformWin()
    {
        NetworkEventSystem.CallEvent(NetworkEventSystem.PLATFORM_WIN_GAME_EVENT);
    }
    bool matchStarted;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            StartMatch();
        }
    }
    void StartMatch()
    {
        if (matchStarted) return;

        matchStarted = true;

        OnStarGame?.Invoke();
        NetworkEventSystem.CallEvent(NetworkEventSystem.START_MATCH_EVENT);
    }
    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
}
