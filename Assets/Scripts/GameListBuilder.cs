using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameListBuilder : MonoBehaviour
{
    private SceneDataList sceneDataList;
    public SceneButtonFactory sceneButtonFactory;
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        sceneDataList = SceneListLoader.LoadSceneList();

        for(int i = 0; i <sceneDataList.list.Count;i++)
            CreateButtonList(sceneDataList.list[i],new Vector3(0,-10*(i+1),0));

        ListSingleton ls = ListSingleton.instance;
        ls.interactableList.ClearList();
        ls.interactableList.FindInteractables();
        
    }

    private Button CreateButtonList(SceneData sceneData, Vector3 position) {
        // TODO Se necessário aumentar o tamanho de content
        Button b = sceneButtonFactory.make(position,parent.transform);
        
        Text t = b.GetComponentInChildren(typeof(Text)) as Text;
        t.text = sceneData.sceneName;
        
        AudioClip ac = Resources.Load<AudioClip>(sceneData.clipPath);
        DescriptionPlayer d = b.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
        d.SetVariables(ac);
        
        return b;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
