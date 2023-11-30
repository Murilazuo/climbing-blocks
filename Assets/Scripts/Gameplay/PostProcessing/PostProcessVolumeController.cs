using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public enum PostProcessTag { WaterDamage}
public class PostProcessVolumeController : MonoBehaviour
{
    
    [SerializeField] PostProcessVolume volume;
    [SerializeField] PostProcessTag postTag;
    public PostProcessVolumeController GetVolumeByTag(PostProcessTag tag)
    {
        if(tag == postTag)
            return this;
        else
            return null;
    }

    public void SetWeight(float value)
    {
        volume.weight = value;
    }

    public void SmothSetWeight(float value, float time)
    {
        LeanTween.value(gameObject, SmothSetWeight, volume.weight, value, time);
    }
}
