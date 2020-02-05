/*
SyncCoordinates.cs
This script updates the Screen's Width & Height, Panel's Width & Height and Panel's position with GaussianBlurV2 shader in the material.
Please Read ReadMe for more info.
*/

/*
ChangeLog
20181803 - Add createNewInstance so users can create a new instance of the material on Awake,
20181803 - Use False values for coordinates in edit mode so people don't have black bars, these black will still show up when playing.

*/


using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GaussianBlur_LiveBlur 
{
    [RequireComponent(typeof(Image))]

    [ExecuteInEditMode]
    public class SyncCoordinates : MonoBehaviour {

        private Material thisMaterial;
        private Image thisImage;
        private Rect thisRect;

        public bool createNewInstance = false;

    #region ScreenHeight
        private float _ScreenHeight;
        public float ScreenHeight
        {
            get{ return _ScreenHeight; }
            set
            {
                if (_ScreenHeight != value)
                {
                    thisMaterial.SetFloat("_ScreenHeight",value);
                    _ScreenHeight = value;
                }
            }
        }
    #endregion

    #region ScreenWidth
        private float _ScreenWidth;
        public float ScreenWidth
        {
            get{ return _ScreenWidth; }
            set
            {
                if (_ScreenWidth != value)
                {
                    thisMaterial.SetFloat("_ScreenWidth",value);
                    _ScreenWidth = value;
                }
            }
        }
    #endregion

    #region PanelY
        private float _PanelY;
        public float PanelY
        {
            get{ return _PanelY; }
            set
            {
                if (_PanelY != value)
                {
                    thisMaterial.SetFloat("_PanelY",value);
                    _PanelY = value;
                }
            }
        }
    #endregion

    #region PanelX
        private float _PanelX;
        public float PanelX
        {
            get{ return _PanelX; }
            set
            {
                if (_PanelX != value)
                {
                    thisMaterial.SetFloat("_PanelX",value);
                    _PanelX = value;
                }
            }
        }
    #endregion

    #region PanelHeight
        private float _PanelHeight;
        public float PanelHeight
        {
            get{ return _PanelHeight; }
            set
            {
                if (_PanelHeight != value)
                {
                    thisMaterial.SetFloat("_PanelHeight",value);
                    _PanelHeight = value;
                }
            }
        }
    #endregion

    #region PanelWidth
        private float _PanelWidth;
        public float PanelWidth
        {
            get{ return _PanelWidth; }
            set
            {
                if (_PanelWidth != value)
                {
                    thisMaterial.SetFloat("_PanelWidth",value);
                    _PanelWidth = value;
                }
            }
        }
    #endregion


        // Use this for initialization
        void Awake () 
        {
            RectTransform thisRectTransform = (RectTransform)transform;
            thisRect = thisRectTransform.rect;

            thisImage = GetComponent<Image>();

            if (!createNewInstance)
            {
                thisMaterial = thisImage.material;
            }
            else
            {
                thisMaterial = Material.Instantiate(thisImage.material);
                thisImage.material = thisMaterial;
            }
            
        }


		private void Update()
		{
#if UNITY_EDITOR

            if (!EditorApplication.isPlaying)
            {
                ScreenHeight = Screen.height;
                ScreenWidth = Screen.width;
                PanelY = float.PositiveInfinity;
                PanelX = float.PositiveInfinity;
                PanelHeight = float.PositiveInfinity;
                PanelWidth = float.PositiveInfinity;

                return;
            }
#endif
		}


		// Update is called once per frame
		void FixedUpdate () 
        {

            RectTransform thisRectTransform = (RectTransform)transform;
            thisRect = thisRectTransform.rect;

            ScreenHeight = Screen.height;
            ScreenWidth = Screen.width;
            PanelY = transform.position.y;
            PanelX = transform.position.x;
            PanelHeight = thisRect.height * transform.lossyScale.y;
            PanelWidth = thisRect.width * transform.lossyScale.x;

        }

    }
}
