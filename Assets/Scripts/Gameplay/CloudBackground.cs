using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBackground : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float maxScale, minScale;
    private void Awake()
    {
        spr.sprite = sprites[Random.Range(0,sprites.Length)];
        transform.localScale = Vector3.one * Random.Range(minScale,maxScale);
    }
}
