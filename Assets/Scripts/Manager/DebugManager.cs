using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [Header("Piece")]
    [SerializeField] TMP_InputField holdTime;
    [SerializeField] TMP_InputField inputDelay, speed, gravity;

    private void Start()
    {
        if (PieceController.Instance)
        {
        }
    }

    /*
    public void SetInputDelay(string text)
    {
        PieceController.Instance.moveDelay = float.Parse(text);
    }
    public void SetHoldTime(string text)
    {
        PieceController.Instance.moveHoldTimeToSpeedMove = float.Parse(text);
    }
    public void SetSpeed(string text)
    {
        PieceController.Instance.moveTimeToMoveSpeed = float.Parse(text);
    }
    public void SetGravity(string text)
    {
        PieceController.Instance.SetGravity(float.Parse(text));
    }
     */

    public void ToggleTimeScale(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
    }
}
