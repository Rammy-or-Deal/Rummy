using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBCardModel : MonoBehaviour
{
    public int id;
    public Material material;
    public Card card;

    public bool isFlipped;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeMaterial(BaccaratCard card)
    {
        string colorCharacter = "";
        switch (card.color)
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
//        LogMgr.Inst.Log(colorCharacter+card.num, (int)LogLevels.PlayerLog1);
//        image.sprite = Resources.Load<Sprite>("Card/"+colorCharacter+card.num);
        Texture tex = Resources.Load<Texture>("Card/"+colorCharacter+card.num);
        material.SetTexture("_MainTex", tex);
    }

    public void FlipOver()
    {
        isFlipped = true;
        transform.localRotation= Quaternion.Euler(90,-180,0);
    }
    
    public void FlipOn()
    {
        isFlipped = false;
        transform.localRotation= Quaternion.Euler(-90,-180,0);
    }
}
