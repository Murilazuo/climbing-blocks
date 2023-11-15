using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Block : MonoBehaviour
{
    int life = 3;
    [SerializeField] SpriteRenderer spr;
    [SerializeField] GameObject particleHit;
    [SerializeField] GameObject particleDeath;
    public void Init(Color color)
    {
        spr.color = color;
    }
    public void Hit()
    {
        life--;
        print("New Life " + life);
        if (life <= 0)
            Die();

        ParticleSystem.MainModule main = Instantiate(particleHit, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().main;

        main.startColor = spr.color;
    }
    void Die()
    {
        Destroy(gameObject);
        ParticleSystem.MainModule main = Instantiate(particleDeath, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().main;

        main.startColor = spr.color;
    }
}
