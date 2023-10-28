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

    public const int START_MATCH_EVENT = 1;
    public const int PIECE_WIN_GAME_EVENT = 2;
    public const int PLATFORM_WIN_GAME_EVENT = 3;
    bool isStartMatch;
    private void Awake()
    {
        instance = this;
    }
    public void PlayerPiecesWin()
    {
        PhotonNetwork.RaiseEvent(PIECE_WIN_GAME_EVENT, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    public void PlayerPlatformWin()
    {
        PhotonNetwork.RaiseEvent(PLATFORM_WIN_GAME_EVENT, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    private void Start()
    {
        StartMatch();
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
        PhotonNetwork.RaiseEvent(START_MATCH_EVENT, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
}
