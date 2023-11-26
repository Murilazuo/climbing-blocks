using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutilineSprite : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] Color color;
    [SerializeField] float offset;
    private void OnValidate()
    {
        Vector2[] positions = {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };


        for(int i = 0; i < 4; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;

            obj.transform.localPosition = positions[i]*offset;
        }
    }
}
