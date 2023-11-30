using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Block : MonoBehaviour
{
    int life = 2;
    [SerializeField] PieceSettings settings;
    [SerializeField] SpriteRenderer spr;
    [SerializeField] GameObject particleHit;
    [SerializeField] GameObject particleDeath;
    public void Init(Color color)
    {
        spr.color = color;
        life = settings.BlockLife;
    }
    public void Hit()
    {
        life--;
        if (life <= 0)
            Die();

        spr.sprite = settings.SpritesBreakBlock[life];

        ParticleSystem.MainModule main = Instantiate(particleHit, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().main;

        main.startColor = spr.color;
    }
    void Die()
    {
        Destroy(gameObject);
        ParticleSystem.MainModule main = Instantiate(particleDeath, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().main;

        main.startColor = spr.color;

        if(PieceController.Instance)
            PieceController.Instance.DestroyBlock(transform.position);
    }
    public void StopPiece()
    {
        spr.sprite = settings.StopBlockSprite;
    }
}
