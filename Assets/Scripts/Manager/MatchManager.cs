using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
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

    public static System.Action OnPlayerPieceWin;
    public static System.Action OnPlayerPlatformWin;

    public static System.Action<Piece> OnSpawnPiece;
    
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Time.timeScale = 1;
    }

    

    public void PlayerPiecesWin()
    {
        OnPlayerPieceWin?.Invoke();
        Time.timeScale = 0;
    }
    public void PlayerPlatformWin()
    {
        Time.timeScale = 0;
        OnPlayerPlatformWin?.Invoke();
    }
    public void GoToMenu()
    {
        EndGamePanel.instance.DisablePanel();
    }
}
