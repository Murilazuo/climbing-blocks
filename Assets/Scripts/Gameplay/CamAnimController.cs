using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CamAnimController : MonoBehaviour
{
    [SerializeField] float timeToMove;
    [SerializeField] float timeToZoom;
    [SerializeField] float finalOrthographicsSize;
    [SerializeField] LeanTweenType ease;
    [SerializeField] float endY;

    float originalSize;
    Vector3 originalPosition;

    private void Start()
    {
        originalSize = Camera.main.orthographicSize;
        originalPosition = transform.position;
    }
    void ResetCam()
    {
        transform.position = originalPosition;
        Camera.main.orthographicSize = originalSize;
    }

    private void OnEnable()
    {
        MatchManager.OnEndGame += OnEndGame;
        MatchManager.OnPlayAgain += ResetCam;
    }
    private void OnDisable()
    {
        MatchManager.OnEndGame -= OnEndGame;
        MatchManager.OnPlayAgain -= ResetCam;
    }
    bool endGame;
    void OnEndGame(int eventCode)
    {
        if (endGame) return;
        endGame = true;

        Vector3 target = transform.position;

        switch (eventCode)
        {
            case NetworkEventSystem.PIECE_COLIDE_WITH_PLATFORM_EVENT:
            case NetworkEventSystem.PLATFORM_REACH_TOP_EVENT:
            case NetworkEventSystem.PLATFORM_DROWNED_EVENT:
                target = FindObjectOfType<PlayerPlatform>().transform.position;
                break;
            case NetworkEventSystem.PIECE_REACH_TOP_EVENT:
                target = Piece.lastPieceStoped.transform.position;
                target.y = endY;
                break;
        }

        target.z = transform.position.z;
        LeanTween.move(gameObject, target, timeToMove).setEase(ease);
        LeanTween.value(gameObject, CamOrthographicSize, Camera.main.orthographicSize, finalOrthographicsSize, timeToZoom);
    }
    private static void CamOrthographicSize(float value)
    {
        Camera.main.orthographicSize = value;
    }
    
}
