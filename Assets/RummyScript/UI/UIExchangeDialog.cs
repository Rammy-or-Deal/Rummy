using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExchangeDialog : MonoBehaviour
{
    public InputField mLeafInput;
    public InputField mGoldInput;
    // Start is called before the first frame update
    void Start()
    {
        mLeafInput.text = "0";
        mGoldInput.text = "0";
    }


    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnClickConvert()
    {
        Debug.Log("convert clicked" + mGoldInput.text);
        if (mLeafInput.text == null)
            return;
        mGoldInput.text = (int.Parse(mLeafInput.text.ToString()) * 1000).ToString();
    }
}
