using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveTween : MonoBehaviour
{
    public float time=0.5f;
    
    int xPos = 5;
    int yPos = -5;
    
    // Start is called before the first frame update
    void Start()
    {
        iTween.MoveBy(gameObject, iTween.Hash("y", yPos,"x", xPos, "loopType", "pingPong","time",time));
//        iTween.MoveTo(gameObject, iTween.Hash("position", pos, "islocal", true, "time", 1));
    }
}
