using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectTeamController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] ButtonChose character;
    [SerializeField] ButtonChose piece;

    [System.Serializable]
    struct ButtonChose
    {
        public Button button;
        public TMP_Text buttonText;
    }
    private IEnumerator Start()
    {
        character.button.interactable = false;
        piece.button.interactable = false;

        if (FindObjectsOfType<PlayerPlatform>().Length > 0)
            DisablePanel();
        else
        {
            ActivePanel();
            yield return new WaitForSeconds(.3f);
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        if (MasterClientManager.Instance.hasPiecePlayer)
            SetFull(piece);
        else
            SetChosable(piece);

        if (MasterClientManager.Instance.hasSpaceToCharacters)
            SetChosable(character);
        else
            SetFull(character);

        SetChosable(character);
    }
    void SetFull(ButtonChose buttonChose)
    {
        buttonChose.buttonText.text = "Full";
        buttonChose.button.interactable = false;
    }
    void SetChosable(ButtonChose buttonChose)
    {
        buttonChose.buttonText.text = "Chose";
        buttonChose.button.interactable = true;
    }
    public void ChangeTeam()
    {
        MasterClientManager.Instance.ClientSetTeam(PlayerType.None);

        ActivePanel();
    }

    void PlayAgain()
    {
        ActivePanel();
    }
    public void ActivePanel()
    {
        UpdateButtons();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }
    void DisablePanel()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }
    public void SelectPlayerType(int playerTypeId)
    {
        MasterClientManager.Instance.ClientSetTeam((PlayerType)playerTypeId);
        DisablePanel();
    }

    public void OnEnable()
    {
        MasterClientManager.OnPlayerSetTeam += UpdateButtons;
        MatchManager.OnPlayAgain += PlayAgain;
    }
    public void OnDisable()
    {
        MasterClientManager.OnPlayerSetTeam -= UpdateButtons;
        MatchManager.OnPlayAgain -= PlayAgain;
    }

    
}
