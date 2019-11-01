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
        if (type == 0)
        {
            circle.color = new Color(66, 135, 39);
            letter.text = "T";
        }
        else if (type == 1)
        {
            circle.color = new Color(236, 50, 63);
            letter.text = "B";
        }
        else if (type == 2)
        {
            circle.color = new Color(50, 101, 236);
            letter.text = "P";
        }

        circle.gameObject.SetActive(true);
        letter.gameObject.SetActive(true);
    }
}
