using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void GuestBtnClick()
    {
        SceneManager.LoadScene("2_Lobby");
//        PunController.Inst.Login();
    }
    public void FaceBookBtnClick()
    {
        SceneManager.LoadScene("2_Lobby");
    }
    
}
