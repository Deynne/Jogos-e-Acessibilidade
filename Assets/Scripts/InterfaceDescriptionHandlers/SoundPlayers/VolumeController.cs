using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour
{

    public DescriptionPlayer descriptionPlayer;

    public volumeSide side;

    public enum volumeSide {
        left,
        right
    };
    private void OnEnable() {
        Slider s = this.gameObject.GetComponent(typeof(Slider)) as Slider;
        if(side == volumeSide.left) {
            float f = PlayerPrefs.GetFloat("leftVolume",1);
            s.value = f;
        }
        else {
            float f = PlayerPrefs.GetFloat("rightVolume",1);
            s.value = f;
        }
    }

    public void ChangeVolumeLeft(System.Single single) {
        descriptionPlayer.ChangeVolumeLeft(single);
        PlayerPrefs.SetFloat("leftVolume",single);
    }

    public void ChangeVolumeRight(System.Single single) {
        descriptionPlayer.ChangeVolumeRight(single);
        PlayerPrefs.SetFloat("rightVolume",single);
    }
}
