using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBRoomManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickBackBtn()
    {
        SceneLoader.LoadScene(Constant.LobbyScene);
    }
}
