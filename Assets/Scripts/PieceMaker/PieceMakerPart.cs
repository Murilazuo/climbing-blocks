using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceMakerPart : MonoBehaviour
{
    public Vector2 position;
    [SerializeField] Image image;
    public bool active = false;

    public void Toggle()
    {
        if (position == Vector2.zero)
            return;

        active = !active;

        image.color = active ? Color.blue : Color.white;
    }
}
