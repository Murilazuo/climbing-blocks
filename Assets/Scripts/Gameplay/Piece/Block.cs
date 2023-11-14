using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Block : MonoBehaviour
{
    int id;
    int life = 3;
    [SerializeField] PhotonView view;
    [SerializeField] SpriteRenderer spr;
    public void Init(int id, Color color)
    {
        spr.color = color;
        this.id = id;
    }
    
    [PunRPC]
    void SetLife(int newLife, int id)
    {
        if (this.id != id) return;

        life = newLife;
        print("New Life " +  life);
        if (life <= 0)
            Die();
    }

    public void Hit()
    {
        view.RPC("SetLife", RpcTarget.All, life - 1, id);
    }
    void Die()
    {
        if(view.IsMine)
        {
            Destroy(gameObject);
        }
    }
}
