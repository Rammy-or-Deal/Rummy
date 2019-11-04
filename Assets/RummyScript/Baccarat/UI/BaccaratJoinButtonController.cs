using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaccaratJoinButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        //Event e = Event.current;
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Hold");
        }
        else
        {
            Debug.Log("Released");
        }

    }
}
