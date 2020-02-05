using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GaussianBlur_Mobile;

public class PausePlay : MonoBehaviour {

    public BlurRenderer_Mobile blurRenderer;

    public int Iterations;
    public int DownRes;

    public Material BlurMaterial;
    public float Lightness;
    public float Saturation;

    public bool isPaused;

    public Animator Anim;

    public Text buttonText;

    // Use this for initialization
    void Start ()
    {
        blurRenderer = BlurRenderer_Mobile.Create();
        blurRenderer.UpdateRate = 0.02f;
        blurRenderer.updateUsingGameTime = false;

        OnPress();
    }


    public void OnPress()
    {
        isPaused = !isPaused;
        Anim.SetBool("isPaused", isPaused);

        buttonText.text = (isPaused) ? "Play" : "Pause";

        StopCoroutine("WhilePlayingAnim");
        StartCoroutine("WhilePlayingAnim");
    }

    public void TimeOn()
    {
        Time.timeScale = 1f;
    }

    public void TimeOff()
    {
        Time.timeScale = 0f;
    }


    //this will play the animation and switch the scene at switchPoint
    private IEnumerator WhilePlayingAnim()
    {

        int i = 0;

        while (i < 10000)
        {
            blurRenderer.Iterations = Iterations;
            blurRenderer.DownRes = DownRes;

            BlurMaterial.SetFloat("_Lightness", Lightness);
            BlurMaterial.SetFloat("_Saturation", Saturation);

            i++;
            yield return 0.2f;
        }


    }


}
