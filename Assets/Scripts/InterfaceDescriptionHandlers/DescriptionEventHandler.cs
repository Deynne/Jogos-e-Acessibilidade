using UnityEngine;
using UnityEngine.EventSystems;

// Um handler personalizado apra lidar com descrições de audio
public interface DescriptionEventHandler  : IEventSystemHandler{
    void OnDescriptorPress(PointerEventData pointerEventData);

}