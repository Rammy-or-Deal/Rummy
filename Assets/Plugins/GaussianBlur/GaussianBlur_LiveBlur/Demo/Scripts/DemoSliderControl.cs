//This Script just joins the value of the silders to the correct blur and lightness values in the materials.
//this script can help you design your own control script for your game.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GaussianBlur_LiveBlur 
{

    public class DemoSliderControl : MonoBehaviour {

        //text and sliders for screen blur
        public Text ScreenBlurText;
        public Slider ScreenBlur;
        public Text ScreenLightnessText;
        public Slider ScreenLightness;

        //text and sliders for panel blur
        public Text PanelBlurText;
        public Slider PanelBlur;
        public Text PanelLightnessText;
        public Slider PanelLightness;

        //text and sliders for world space panel blur
        public Text WSPanelBlurText;
        public Slider WSPanelBlur;
        public Text WSPanelLightnessText;
        public Slider WSPanelLightness;

        //materials to control
        public Material ScreenBlurLayer;
        public Material PanelBlurLayer;
        public Material WSPanelBlurLayer;


        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update () 
        {
            ScreenBlurLayer.SetFloat("_BlurSize",ScreenBlur.value);
            ScreenBlurText.text = "BlurSize: " + ScreenBlur.value.ToString("F3");

            ScreenBlurLayer.SetFloat("_Lightness",ScreenLightness.value);
            ScreenLightnessText.text = "Lightness: " + ScreenLightness.value.ToString("F3");

            PanelBlurLayer.SetFloat("_BlurSize",PanelBlur.value);
            PanelBlurText.text = "BlurSize: " + PanelBlur.value.ToString("F3");

            PanelBlurLayer.SetFloat("_Lightness",PanelLightness.value);
            PanelLightnessText.text = "Lightness: " + PanelLightness.value.ToString("F3");

            WSPanelBlurLayer.SetFloat("_BlurSize",WSPanelBlur.value);
            WSPanelBlurText.text = "BlurSize: " + WSPanelBlur.value.ToString("F3");

            WSPanelBlurLayer.SetFloat("_Lightness",WSPanelLightness.value);
            WSPanelLightnessText.text = "Lightness: " + WSPanelLightness.value.ToString("F3");
        }

        //reset the values
        public void Reset()
        {
            ScreenBlur.value = 0f;
            ScreenLightness.value  = 0f;

            PanelBlur.value = 5f;
            PanelLightness.value = 0.2f;

            WSPanelBlur.value = 50f;
            WSPanelLightness.value = -0.25f;
        }
    }
}
