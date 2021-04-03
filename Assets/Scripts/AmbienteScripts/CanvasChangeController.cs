using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Em casos onde há mudança entre as "telas" de uma scene, realiza o carregamento da nova tela.
public class CanvasChangeController : MonoBehaviour
{
    // Salva a instância da Canvas (A tela) atualmente ativo. A ideia é que todo botão de mudança de canvas tenha a
    // referêcia para o seu canvas.
    #pragma warning disable CS0649
    [SerializeField]
    private GameObject currentCanvas;
    #pragma warning restore CS0649
    // Realiza a mudança do canvas, desativando o atual e desativando o novo.
    public void changeCanvas(GameObject newCanvas) {
        // Troca o canvas ativo
        currentCanvas.SetActive(false);
        newCanvas.SetActive(true);

        // Obtêm o novo objeto que tem o foco atualmente.
        GameObject g = ListSingleton.instance.focusedGo;

        // Define ele como selecionado (Preciso fazer isso aqui ou ele não fica "aceso" na tela.)
        Selectable s = g.GetComponent(typeof(Selectable)) as Selectable;
        s.Select();

        // Toca a descrição o primeiro botão selecionado na nova tela.
        DescriptionPlayer dp = ListSingleton.instance.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
        dp.OnDescriptorPress(null);
    }
}
