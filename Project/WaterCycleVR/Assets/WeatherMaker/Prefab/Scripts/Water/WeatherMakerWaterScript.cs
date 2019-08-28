//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset off of leak forums or any other horrible evil pirate site, please consider buying it from the Unity asset store at https ://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections.Generic;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Water rendering modes
    /// </summary>
    public enum WeatherMakerWaterRenderingMode
    {
        /// <summary>
        /// Render water in a single pass with all lights
        /// </summary>
        OnePass,

        /// <summary>
        /// Render water in a forward base + forward add pass
        /// </summary>
        ForwardBasePlusAdd
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WeatherMakerWaterScript : MonoBehaviour
    {
        [Tooltip("Water rendering mode")]
        public WeatherMakerWaterRenderingMode RenderMode = WeatherMakerWaterRenderingMode.OnePass;

        [Tooltip("Whether to blend to allow smooth transition as depth decreases.")]
        public bool EnableDepthBlend = true;

        [Tooltip("Enable depth write for water surface. The depth buffer is this many units below the water. 0 for no depth write. " +
            "Turn this on if you have fog or other depth effects over deep water.")]
        [Range(0.0f, 10000.0f)]
        public float WaterDepthThreshold = 0.0f;

        public MeshRenderer MeshRenderer { get; private set; }
        private WeatherMakerReflectionScript reflection;

        protected virtual void UpdateShader()
        {
            if (RenderMode == WeatherMakerWaterRenderingMode.OnePass)
            {
                MeshRenderer.sharedMaterial.shader.maximumLOD = 201;
            }
            else
            {
                MeshRenderer.sharedMaterial.shader.maximumLOD = 101;
            }

            MeshRenderer.sharedMaterial.SetFloat("_WaterDepthThreshold", (WaterDepthThreshold <= 0.0f ? float.MinValue : WaterDepthThreshold));

            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) && EnableDepthBlend)
            {
                MeshRenderer.sharedMaterial.EnableKeyword("WATER_EDGEBLEND_ON");
            }
            else
            {
                EnableDepthBlend = false;
                MeshRenderer.sharedMaterial.DisableKeyword("WATER_EDGEBLEND_ON");
            }

            if (reflection != null && reflection.enabled)
            {
                MeshRenderer.sharedMaterial.EnableKeyword("WATER_REFLECTIVE");
            }
            else
            {
                MeshRenderer.sharedMaterial.DisableKeyword("WATER_REFLECTIVE");
            }

            MeshRenderer.sharedMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        }

        protected virtual void AdjustScaleAndPosition()
        {
            // ensure y scale is always 1, otherwise problems arrise
            Vector3 scale = transform.localScale;
            scale.y = 1.0f / transform.parent.lossyScale.y;
            transform.localScale = scale;
        }

        protected virtual void OnEnable()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            reflection = GetComponent<WeatherMakerReflectionScript>();
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Update()
        {
            UpdateShader();
            AdjustScaleAndPosition();
        }

        protected virtual void OnWillRenderObject()
        {
            Camera currentCamera = Camera.current;
            if (currentCamera == null)
            {
                return;
            }
            else if (EnableDepthBlend && (currentCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
            {
                currentCamera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }
    }
}