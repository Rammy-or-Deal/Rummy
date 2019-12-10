using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaccaratRoomMoneyButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Image cover;
    public bool isSelected = false;
    public BaccaratRoomMoneyButton friend;
    void Start()
    {
        UpdateMe();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickedMe()
    {
        isSelected = true;
        friend.isSelected = !isSelected;
        UpdateMe();
        friend.UpdateMe();
    }
    public void UpdateMe()
    {
        cover.gameObject.SetActive(isSelected);
    }
}
