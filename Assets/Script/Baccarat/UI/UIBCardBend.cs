using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class UIBCardBend : MonoBehaviour,IPunOwnershipCallbacks
{
    // Start is called before the first frame update
    public Vector3 lastPoint;
    public Transform[] bend;
    public UIBCardModel[] cards;
    public int id;
    public bool isClicked;
    private float damping = 10;
    public static UIBCardBend Inst;

    public Transform bigCamPos;
    public Transform originCamPos;
    
    [HideInInspector]
    public PhotonView photonView;
   
    void Start()
    {
        Inst = this;
        photonView = GetComponent<PhotonView>();
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
                        if (dis<0.15)
                            break;
//                        Debug.Log(dis);
                        UIBCardModel card= hit.collider.GetComponent<UIBCardModel>();
                        if ((!card || (card && card.id!=id)) && dis>0.5)  //turn Card
                        {
                            TouchEnd();
                            cards[id].FlipOver();
                            //BaccaratPanMgr.Inst.OnClickDistributedCard();
                            return;
                        }
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
                    TouchEnd();
                }
                break;
        }
    }

    void TouchEnd()
    {
        bend[id].localPosition = new Vector3(2,0,0);
        bend[id].localRotation=new Quaternion(0,0,0,0);
        isClicked = false;
    }

    public void ShowBigCard(bool isBigShow)
    {
        float time = 0.5f;
        gameObject.SetActive(isBigShow);
        BaccaratUIController.Inst.bendCardBlankBtn.SetActive(isBigShow);
        if (isBigShow)
            iTween.MoveTo(BaccaratUIController.Inst.camera, bigCamPos.position, time);
        else
            BaccaratUIController.Inst.camera.transform.localPosition=new Vector3(0,0,0);
    }

    public void ShowBigCard(Transform[] destination_cardPos, BaccaratCard card1, BaccaratCard card2)
    {
        if (!photonView.IsMine) { photonView.RequestOwnership(); }
        cards[0].ChangeMaterial(card1);
        cards[1].ChangeMaterial(card2);
        StartCoroutine(ShowCard(destination_cardPos));
    }
    
    IEnumerator ShowCard(Transform[] destination_cardPos)
    {
        yield return new WaitForSeconds(Constants.BaccaratDistributionTime);
        for (int i=0;i<cards.Length;i++)
            cards[id].FlipOn();
        transform.position = destination_cardPos[0].position;
        ShowBigCard(true);
    }


    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.LogError("OnOwnershipRequest");
        Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + targetView + ".");
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.LogError("OnOwnershipTransfered");
        Debug.Log("OnOwnershipTransfered(): Player " + previousOwner + " requests ownership of: " + targetView + ".");
        throw new System.NotImplementedException();
    }
}
