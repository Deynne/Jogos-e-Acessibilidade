using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!(PlayerPrefs.GetInt("showInterface",1) == 1)) {
            Component[] graphics = GetComponentsInChildren(typeof(Graphic));
            if(graphics != null) {
                foreach (var graphic in graphics)
                {
                    if(!graphic.CompareTag("Pointer"))
                        ((Graphic) graphic).enabled = false;    
                }
                
            }

            graphics = GetComponentsInChildren(typeof(Renderer));

            if(graphics != null) {
                foreach (var graphic in graphics)
                {
                    if(!graphic.CompareTag("Pointer"))
                        ((Renderer) graphic).enabled = false;    
                }
                
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckGraphics() {
        Component[] graphics = GetComponentsInChildren(typeof(Graphic));
        if(graphics != null) {
            if(!(PlayerPrefs.GetInt("showInterface",1) == 1)) {
                foreach (var graphic in graphics) {
                    if(!graphic.CompareTag("Pointer"))
                        ((Graphic) graphic).enabled = false;    
                }
                    
            }
            else {
                foreach (var graphic in graphics) {
                    if(!graphic.CompareTag("Pointer"))
                        ((Graphic) graphic).enabled = true;    
                }
            }
        }

        graphics = GetComponentsInChildren(typeof(Renderer));

        if(graphics != null) {
            if(!(PlayerPrefs.GetInt("showInterface",1) == 1)) {
                foreach (var graphic in graphics) {
                    if(!graphic.CompareTag("Pointer"))
                        ((Renderer) graphic).enabled = false;    
                }
                    
            }
            else {
                foreach (var graphic in graphics) {
                    if(!graphic.CompareTag("Pointer"))
                        ((Renderer) graphic).enabled = true;    
                }
            }
        }
    }
}
