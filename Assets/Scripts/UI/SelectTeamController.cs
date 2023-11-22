using ExitGames.Client.Photon;
using Photon.Pun;
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

        ActivePanel();
        yield return new WaitForSeconds(.3f);
        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (MatchManager.Instance.HasPiecePlayer)
            SetFull(piece);
        else
            SetChosable(piece);

        if (MatchManager.Instance.HasSpaceToCharacters)
            SetChosable(character);
        else
            SetFull(character);
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
    public void ActivePanel()
    {
        MatchManager.Instance.SelectPlayerType(PlayerType.None);
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
        MatchManager.Instance.SelectPlayerType((PlayerType)playerTypeId);
        DisablePanel();
    }

    void OnReceiveNetworkEvent(EventData eventData)
    {
        if(eventData.Code == NetworkEventSystem.UPDATE_PLAYERS_SELECT_EVENT)
            UpdateButtons();
    }
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnReceiveNetworkEvent;
    }
    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnReceiveNetworkEvent;
    }

}
