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
        endGame = false;
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
    void OnEndGame(int eventCode,Vector2 position)
    {
        if (endGame) return;
        endGame = true;

        Vector3 target = position;

        target.z = transform.position.z;
        LeanTween.move(gameObject, target, timeToMove).setEase(ease);
        LeanTween.value(gameObject, CamOrthographicSize, Camera.main.orthographicSize, finalOrthographicsSize, timeToZoom);
    }
    private static void CamOrthographicSize(float value)
    {
        Camera.main.orthographicSize = value;
    }
    
}
