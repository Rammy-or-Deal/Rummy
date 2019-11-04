using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class UIBCardBend : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 lastPoint;
    public Transform[] bend;
    public Transform[] card;
    public int id;
    public bool isClicked;
    private float damping = 10;
    public static UIBCardBend Inst;

    public GameObject bankBtn;
    public GameObject camera;
    public Transform bigCamPos;
    public Transform originCamPos;

    private bool isBigShow;
    
    void Start()
    {
        Inst = this;
    }
    
    void Update () {
        // Handle native touch events
        foreach (Touch touch in Input.touches) {
            HandleTouch(touch.fingerId,touch.position, touch.phase);
        }

        // Simulate touch events from mouse events
        if (Input.touchCount == 0) {
            if (Input.GetMouseButtonDown(0) ) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Began);
            }
            if (Input.GetMouseButton(0) ) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0) ) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
            }
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase) {
        switch (touchPhase) {
            case TouchPhase.Began:
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out hit, 100))
                {
                    UIBCardModel card= hit.collider.GetComponent<UIBCardModel>();
                    if (card)
                    {
                        Debug.Log(hit.point+card.id.ToString());
                        id = card.id;
                        lastPoint = hit.point;
                        isClicked = true;
                    }
                }
                break;
            case TouchPhase.Moved:
                if (isClicked)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out hit, 100))
                    {
                        
                        Vector3 relativePos = hit.point-lastPoint;
                        float dis = relativePos.magnitude;
                        if (dis<0.2)
                            break;
//                        Debug.Log(dis);
                        bend[id].position = hit.point;
//                        Debug.Log(hit.point);
                        // the second argument, upwards, defaults to Vector3.up
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        rotation *= Quaternion.Euler(0, 90, 0);
                        bend[id].rotation = rotation;// Quaternion.Slerp(bend[id].rotation, rotation, Time.deltaTime * damping);
                    }
                }
                break;
            case TouchPhase.Ended:
                if (isClicked)
                {
                    bend[id].localPosition = new Vector3(2,0,0);
                    bend[id].localRotation=new Quaternion(0,0,0,0);
                    isClicked = false;
                }
                break;
        }
    }

    public void OnClickShowBigCard()
    {
        float time = 0.5f;
        isBigShow = !isBigShow;
        gameObject.SetActive(isBigShow);
        bankBtn.SetActive(isBigShow);
        if (isBigShow)
            iTween.MoveTo(camera, bigCamPos.position, time);
        else
            camera.transform.localPosition=new Vector3(0,0,0);
            
    }
}
