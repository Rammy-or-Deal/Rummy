using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAvatarDialog : MonoBehaviour
{
    public UIAvatarItem[] avatarItems;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < avatarItems.Length; i++)
        {
            avatarItems[i].pic.sprite = Resources.Load<Sprite>("new_avatar/avatar_" + (i + 1));

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClose(GameObject obj)
    {
        obj.SetActive(false);
    }

}
