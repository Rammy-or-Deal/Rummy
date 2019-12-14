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

    public void FlipOver()
    {
        isFlipped = true;
        transform.localRotation= Quaternion.Euler(90,-180,0);
    }
}
