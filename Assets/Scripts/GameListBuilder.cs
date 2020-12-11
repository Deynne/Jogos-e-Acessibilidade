﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            CreateButton(sceneDataList.list[i],new Vector3(0,-10*(i+1),0));

        ListSingleton ls = ListSingleton.instance;
        ls.interactableList.ClearList();
        ls.interactableList.FindInteractables();
        ls.interactableList.Next();
        EventSystem.current.SetSelectedGameObject(ls.interactableList.focusedGo);
        DescriptionPlayer dp = ls.interactableList.focusedGo.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
        dp.OnDescriptorPress(null);
    }

    private Button CreateButton(SceneData sceneData, Vector3 position) {
        // TODO Se necessário aumentar o tamanho de content
        Button b = sceneButtonFactory.make(Vector3.zero,parent.transform);
        b.transform.localPosition = position;
        Text t = b.GetComponentInChildren(typeof(Text)) as Text;
        t.text = sceneData.sceneName;
        
        AudioClip ac = Resources.Load<AudioClip>(sceneData.clipPath);
        DescriptionPlayer d = b.GetComponent(typeof(DescriptionPlayer)) as DescriptionPlayer;
        d.SetVariables(ac);

        b.onClick.AddListener(() => {
                                        SceneChanger c = (SceneChanger) b.gameObject.AddComponent(typeof(SceneChanger));
                                        c.LoadGame(sceneData.scenePath);
                                    });
        
        return b;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
