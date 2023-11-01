using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explosionSize;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] PhotonView view;
    [SerializeField] TMP_Text text;
    [SerializeField] int secondsToExplode;

    public static Action OnBombExplode;
    private void Start()
    {
        LeanTween.delayedCall(1, PassOneSecond);
        text.text = secondsToExplode.ToString();
    }

    void PassOneSecond()
    {
        secondsToExplode--;

        if (secondsToExplode == 0)
            LeanTween.delayedCall(.5f, Explode);
        else
            LeanTween.delayedCall(1, PassOneSecond);

        text.text = secondsToExplode.ToString();
    }
    void Explode()
    {
        OnBombExplode?.Invoke();


        foreach(var coll in Physics2D.OverlapCircleAll(transform.position, explosionSize))
            MatchManager.Instance.DestroyBlock(coll.gameObject);

        if(view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionSize);
    }
}
