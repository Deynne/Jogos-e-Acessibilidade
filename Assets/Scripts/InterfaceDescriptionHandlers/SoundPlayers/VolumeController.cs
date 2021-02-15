using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{

    public DescriptionPlayer descriptionPlayer;


    public void ChangeVolumeLeft(System.Single single) {
        descriptionPlayer.ChangeVolumeLeft(single);
    }

    public void ChangeVolumeRight(System.Single single) {
        descriptionPlayer.ChangeVolumeRight(single);
    }
}
