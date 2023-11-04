using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Burst.CompilerServices;
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
    public static System.Action OnStarCounter;
    public static System.Action<int> OnEndGame;
    
    
    private void Awake()
    {
        instance = this;
    }
    public void PlayerPiecesWin()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PIECE_WIN_GAME_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_WIN_GAME_EVENT);
    }
    public void PlayerPlatformWin()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PLATFORM_WIN_GAME_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PLATFORM_WIN_GAME_EVENT);
    }
    bool matchStarted = false;
    public void StartMatch()
    {
        if (matchStarted) return;

        matchStarted = true;
        OnStarGame?.Invoke();
        NetworkEventSystem.CallEvent(NetworkEventSystem.START_MATCH_EVENT);
    }
    public void StartCounter()
    {
        OnStarCounter?.Invoke();
        NetworkEventSystem.CallEvent(NetworkEventSystem.START_COUNTER_EVENT);
    }
    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

    public void DestroyBlock(GameObject block)
    {
        if (block.CompareTag(Piece.STOPED_PIECE_TAG))
        {
            object[] data = { block.transform.position };

            DestroyImmediate(block);

            NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_DESTROY_EVENT, data);
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEndGameEvent;
    }
    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEndGameEvent;
    }
    void OnEndGameEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case NetworkEventSystem.PLATFORM_WIN_GAME_EVENT:
            case NetworkEventSystem.PIECE_WIN_GAME_EVENT:
                OnEndGame?.Invoke(eventData.Code);
                break;
            case NetworkEventSystem.START_MATCH_EVENT:
                if (!matchStarted)
                {
                    matchStarted = true;
                    OnStarGame?.Invoke();
                }
                break;
            case NetworkEventSystem.START_COUNTER_EVENT:
                OnStarCounter?.Invoke();
                break;
        }

    }
}
