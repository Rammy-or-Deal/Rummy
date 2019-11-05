using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class UIBHistoryCell : MonoBehaviour
{
    public Image circle;
    public Text letter;
    public int id;
    public int type;
    
    void Start()
    {
        
    }

    public void UpdateInfo(int type)
    {
        this.type = type;
        if (type == Constants.BaccaratDrawArea)
        {
            circle.color = new Color32(66, 135, 39, 200);
            letter.text = "T";
        }
        else if (type == Constants.BaccaratBankerArea)
        {
            circle.color = new Color32(236, 50, 63, 200);
            letter.text = "B";
        }
        else if (type == Constants.BaccaratPlayerArea)
        {
            circle.color = new Color32(50, 101, 236, 200);
            letter.text = "P";
        }

        
        circle.gameObject.SetActive(type != -1);
        letter.gameObject.SetActive(type != -1);


    }
}
