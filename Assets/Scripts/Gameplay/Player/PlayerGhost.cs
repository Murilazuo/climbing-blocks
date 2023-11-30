using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDeathId { Drow, Smash}
public class PlayerGhost : MonoBehaviour
{
    [SerializeField] float toMove;
    [SerializeField] float timeMove;
    [SerializeField] float timeToFade;
    [SerializeField] float timeToDisable;
    [SerializeField] PhotonView photonView;
    [SerializeField] LeanTweenType ease;
    [SerializeField] GameObject render;
    [SerializeField] PhotonView view;

    bool IsLastCharcter { get => FindObjectsOfType<PlayerPlatform>().Length == 0; }
    public void Init(PlayerDeathId deathId)
    {
        LeanTween.delayedCall(toMove / 2, () => CheckDeath(deathId));

        view.RPC(nameof(FadeColor), RpcTarget.All);

        LeanTween.moveY(gameObject, transform.position.y + toMove, timeMove)
            .setEase(ease);
    }

    [PunRPC]
    void FadeColor()
    {
        LeanTween.color(render, Color.clear, timeToFade)
            .setDelay(toMove)
            .setOnComplete(() =>
            {
                if (photonView.IsMine)
                    PhotonNetwork.Destroy(gameObject);
            });
    }
    void CheckDeath(PlayerDeathId deathId)
    {
        if (IsLastCharcter)
        {
            switch (deathId)
            {
                case PlayerDeathId.Drow:
                    MatchManager.Instance.PlayerDrowned(transform.position);
                    break;
                case PlayerDeathId.Smash:
                    MatchManager.Instance.PieceCollideWithPieceReachTop(transform.position);
                    break;
            }
        }
    }

}
