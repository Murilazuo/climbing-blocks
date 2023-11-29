using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPreferredHeight : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    public void UpdateHeight()
    {
        rect.sizeDelta = new(rect.sizeDelta.x, verticalLayoutGroup.preferredHeight);
    }
}
