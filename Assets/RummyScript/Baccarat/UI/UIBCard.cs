using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBCard : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }
    public void Init()
    {
        this.gameObject.SetActive(false);
        //"new_avatar/avatar_" + Random.Range(1,26).ToString();
        image.sprite = Resources.Load<Sprite>("Card/_black");
    }
    public void ShowImage(int num, int col)
    {
        this.gameObject.SetActive(true);
        string colorCharacter = "";
        switch (col)
        {
            case 0:
                colorCharacter = "A"; break;
            case 1:
                colorCharacter = "B"; break;
            case 2:
                colorCharacter = "C"; break;
            case 3:
                colorCharacter = "D"; break;
            default:
                colorCharacter = "A"; break;
        }
        image.sprite = Resources.Load<Sprite>("Card/"+colorCharacter+num);
    }

    public void OnClickCard()
    {
        
    }
    
}
