/*
BlurRenderer_Mobile.cs
Creates global textures that have been passed through the GaussianBlur_Mobile(Hidden).shader
*/

using UnityEngine;


namespace GaussianBlur_Mobile
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class BlurRenderer : MonoBehaviour
    {


        [Range(0, 25)]
        /// <summary>
        /// how many Iterations should the blur be applied for
        /// </summary>
        public int Iterations;

        [Range(0, 5)]
        /// <summary>
        /// Lowers the resolution of the texture, thus allowing for a larger blur without so many iterations
        /// </summary>
        public int DownRes;

        /// <summary>
        /// Weather to update the blur
        /// </summary>
        public bool UpdateBlur = true;

        /// <summary>
        /// amount of time that will pass before re-rendering the blur
        /// </summary>
        public float UpdateRate = 0.02f;

        /// <summary>
        /// last time we rendered the blur
        /// </summary>
        private float lastUpdate = 0.0f;

        /// <summary>
        /// Stores the blur texture between renders
        /// </summary>
        public RenderTexture BlurTexture;

        /// <summary>
        /// The material where we'll pass the screen texture though to create the blur
        /// </summary>
        private Material mat;



        private void Start()
        {
            //set up the blur texture

            BlurTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);

            /*
            //we can swap the code above for a texture that will take up less ram...if needed.
            BlurTexture = new RenderTexture(128, 128, 4, RenderTextureFormat.ARGB4444);
            */

            //create the texture
            BlurTexture.Create();

            //create our material if we have not already
            if (mat == null)
            {
                mat = new Material(Shader.Find("Hidden/GaussianBlur_Mobile"));
            }
        }


        #region BlurScaleSlider

        //this section will enable a slider that controls both the Iterations and DownRes
        //Uncomment the code below to use it

        /*
        [Range(0, 100)]
        /// <summary>
        /// The blur scale.
        /// </summary>
        public int blurScale;

        private void Update()
        {
            int tempBlurScale = blurScale;

            Iterations = 0;
            DownRes = 0;

            while (tempBlurScale > 20 && DownRes < 5)
            {
                DownRes += 1;
                tempBlurScale -= 15;
            }

            Iterations = tempBlurScale;

            tempBlurScale = 0;
        }
        */

        #endregion

        //OnRenderImage will execute before each frame renders
        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            // if we need to re-render
            if (Time.time - lastUpdate >= UpdateRate
                && UpdateBlur)
            {
                //set the width and height
                int width = src.width >> DownRes;
                int height = src.height >> DownRes;

                //create a temp texture
                RenderTexture rt = RenderTexture.GetTemporary(width, height);

                //move the screen image to our temp texture
                Graphics.Blit(src, rt);

                //loop to add the blur to our temp texture
                for (int i = 0; i < Iterations; i++)
                {
                    RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
                    Graphics.Blit(rt, rt2, mat);
                    RenderTexture.ReleaseTemporary(rt);
                    rt = rt2;
                }

                //store our texture in BlurTexture
                Graphics.Blit(rt, BlurTexture);

                //set the global texture for our shader to use
                Shader.SetGlobalTexture("_MobileBlur", rt);

                //remove the temp texture
                RenderTexture.ReleaseTemporary(rt);

                //set lastUpdate
                lastUpdate = Time.time;

            }
            else
            {
                //set the global texture again...must be done for each frame
                Shader.SetGlobalTexture("_MobileBlur", BlurTexture);
            }

            //make sure the camera renders as normal.
            Graphics.Blit(src, dst);

        }



        /// <summary>
        /// Creates a BlurRenderer_Mobile on the main camera
        /// </summary>
        static public BlurRenderer Create()
        {
            BlurRenderer BRM = Camera.main.gameObject.GetComponent<BlurRenderer>();

            if (BRM == null)
            {
                BRM = Camera.main.gameObject.AddComponent<BlurRenderer>();
            }

            return BRM;
        }

        //override to allow different camera
        static public BlurRenderer Create(Camera ThisCamera)
        {

            BlurRenderer BRM = ThisCamera.gameObject.GetComponent<BlurRenderer>();

            if (BRM == null)
            {
                BRM = ThisCamera.gameObject.AddComponent<BlurRenderer>();
            }

            return BRM;
        }

    }
}
