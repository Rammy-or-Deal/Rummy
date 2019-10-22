using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveTween : MonoBehaviour
{
    public float time=0.5f;
    public int yPos = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        iTween.MoveBy(gameObject, iTween.Hash("y", yPos, "loopType", "pingPong","time",time));
//        iTween.MoveTo(gameObject, iTween.Hash("position", pos, "islocal", true, "time", 1));
    }
}
