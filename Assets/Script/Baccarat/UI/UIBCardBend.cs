using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

public class UIBCardBend : MonoBehaviour,IPunOwnershipCallbacks
{
    //card position issue (other side) on center.
    //blend frame position issue due to sync 
    public Vector3 lastPoint;
    public Transform[] bend;
    public UIBCardModel[] cards;
    public int id;
    public bool isClicked;
    private float damping = 10;
    private int flippedCnt = 0;

    public GameObject camera;
    public Transform bigCamPos;
    
    Vector3 _originCamPos;
    private bool isController;
    UIBCard[] originCards;
    
    [HideInInspector]
    public PhotonView photonView;
   
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        _originCamPos = camera.transform.position;
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
        if (!isController)
            return;
        switch (touchPhase) {
            case TouchPhase.Began:
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out hit, 100))
                {
                    UIBCardModel card= hit.collider.GetComponent<UIBCardModel>();
                    if (card && !card.isFlipped)
                    {
//                        Debug.Log(hit.point+card.id.ToString());
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
                            FlipCard();
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

    void FlipCard()
    {
        TouchEnd();
        photonView.RPC("FlipOver", RpcTarget.All,id);
        flippedCnt++;
        if (flippedCnt == 2)
        {
            StartCoroutine(HideBigCard());
        }
    }

    IEnumerator HideBigCard()
    {
        yield return new WaitForSeconds(0.8f);
        iTween.MoveTo(camera, _originCamPos, 0.8f);
        yield return new WaitForSeconds(0.8f);
//        ShowBigCard(false);  //this will be run automatically 
    }

    private int isFirst;
    
    public void ShowBigCard(Transform[] destination_cardPos, BaccaratCard card1, BaccaratCard card2,bool isController,UIBCard[] orgCards,int isFirst)
    {
        originCards = orgCards;
        this.isFirst = isFirst;
        
        
        this.isController = isController;
        if (isController && !photonView.IsMine) {
            photonView.RequestOwnership();
            for (int i = 0; i <= isFirst; i++)
            {
                bend[i].GetComponent<PhotonView>().RequestOwnership();    
            }
        }
        if (isController)
            photonView.RPC("ChangeMaterial", RpcTarget.All, card1.color,card1.num,card2.color,card2.num);

        if (isFirst == 1)
        {
            cards[0].transform.parent.localPosition=new Vector3(-0.7f,1,0);
            cards[1].transform.parent.localPosition=new Vector3(0.7f,1,0);
        }
        else
        {
            cards[0].transform.parent.localPosition=new Vector3(0,1,0);
            cards[1].transform.parent.localPosition=new Vector3(-1000,1,0);
        }
        flippedCnt = 1-isFirst;
        StartCoroutine(ShowCard(destination_cardPos));
    }
    
    IEnumerator ShowCard(Transform[] destination_cardPos)
    {
        yield return new WaitForSeconds(Constants.BTweenTime);
        FlipOn();
        transform.position = destination_cardPos[0].parent.position;
        if (isController)
        {
            BaccaratUIController.Inst.bendCardBlankBtn.SetActive(true);
            iTween.MoveTo(camera, bigCamPos.position, 0.5f);
        }
        ShowSmallCard(false);
        if (isFirst==1)
            yield return new WaitForSeconds(Constants.BSqueezeWaitTime-Constants.BTweenTime);
        else
            yield return new WaitForSeconds(Constants.B3rdWaitTime-Constants.BTweenTime);
        if (isController)
        {
            BaccaratUIController.Inst.bendCardBlankBtn.SetActive(false);
            camera.transform.localPosition=new Vector3(0,0,0);
        }
        transform.position = new Vector3(0,0,0);
        TouchEnd();
    }

    [PunRPC]
    void ShowSmallCard(bool isFlag)
    {
        if (originCards == null)
        {
            Debug.LogError("no originCards");
            return;
        }

        if (isFirst == 1)
        {    
            originCards[0].gameObject.SetActive(isFlag);
            originCards[1].gameObject.SetActive(isFlag);    
        }
        else
        {
            originCards[2].gameObject.SetActive(isFlag);
        }
    }
    
    [PunRPC]
    void ChangePosition(Vector3 pos)
    {
        transform.position = pos;
    }
    [PunRPC]
    void ChangeMaterial(int col0,int num0,int col1,int num1)
    {
        cards[0].ChangeMaterial(col0,num0);
        cards[1].ChangeMaterial(col1,num1);
    }

    [PunRPC]
    void FlipOn()
    {
        for (int i=0;i<cards.Length;i++)
            cards[i].FlipOn();
    }
    
    [PunRPC]
    void FlipOver(int id0)
    {
        cards[id0].FlipOver();        
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
