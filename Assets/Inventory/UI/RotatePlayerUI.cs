using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class RotatePlayerUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public float rotateSpeed = 0.1f;

    private Vector3 lastMousePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Set default mouse position so it can be used to calculate the difference when dragging.
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Input.GetMouseButton(1))
        {
            //Find rotation amount  by using difference in mouse position. We only need to modify the y rotation (left/right)
            Vector3 currentMousePosition = eventData.position;
            Vector3 delta = currentMousePosition - lastMousePosition;
            Vector3 rotation = new Vector3(0, -delta.x, 0) * rotateSpeed;
            GameObject.Find("UICharacter").transform.Rotate(rotation);

            //Update the mouse position while dragging
            lastMousePosition = eventData.position;
        }
    }
}
