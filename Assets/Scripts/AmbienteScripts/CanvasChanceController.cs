using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChanceController : MonoBehaviour
{
    [SerializeField]
    private GameObject currentCanvas;
    public void changeCanvas(GameObject newCanvas) {
        currentCanvas.SetActive(false);
        newCanvas.SetActive(true);

        GameObject g = ListSingleton.instance.focusedGo;

        Selectable s = g.GetComponent(typeof(Selectable)) as Selectable;
        s.Select();
        DescriptionPlayer dp = ListSingleton.instance.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
        dp.OnDescriptorPress(null);
    }
}
