using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaccaratJoinButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    public UIBRoomItem parent;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool buttonPressed;
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        Debug.Log("Left Hold. status=" + parent.baccaratRoomInfo.status);
        UIBHistory.Inst.ParseStatusString(parent.baccaratRoomInfo.status);
        iTween.MoveTo(UIBHistory.Inst.gameObject, this.gameObject.transform.position, 0);
        UIBHistory.Inst.gameObject.SetActive(true);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
        Debug.Log("Left Released");

        UIBHistory.Inst.gameObject.SetActive(false);
    }
}
