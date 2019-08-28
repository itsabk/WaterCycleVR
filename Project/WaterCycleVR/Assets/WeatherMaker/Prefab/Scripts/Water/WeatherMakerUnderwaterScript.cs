using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(WeatherMakerNullFogZoneScript))]
    public class WeatherMakerUnderwaterScript : WeatherMakerWaterScript
    {
        public WeatherMakerWaterScript SurfaceScript;

        private WeatherMakerNullFogZoneScript nullFogZone;
        private readonly HashSet<Camera> underwaterCameras = new HashSet<Camera>();
        private BoxCollider underwaterCollider;

        protected override void UpdateShader()
        {
            base.UpdateShader();

            MeshRenderer.sharedMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (underwaterCollider == null)
            {
                underwaterCollider = GetComponent<BoxCollider>();
                nullFogZone = GetComponent<WeatherMakerNullFogZoneScript>();
                MeshRenderer.enabled = false;
                nullFogZone.enabled = false;

#if UNITY_EDITOR

                if (Application.isPlaying)
                {

#endif

                    MeshRenderer.sharedMaterial = MeshRenderer.material;

#if UNITY_EDITOR

                }

#endif

                MeshRenderer.sharedMaterial.DisableKeyword("WATER_REFLECTIVE");
                MeshRenderer.sharedMaterial.EnableKeyword("WATER_UNDERWATER");
                MeshRenderer.sharedMaterial.renderQueue = 2501;
            }

            Camera.onPreCull += CameraPreCull;
            Camera.onPostRender += CameraPostRender;
        }

        protected override void AdjustScaleAndPosition()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            underwaterCameras.Clear();
            Camera.onPreCull -= CameraPreCull;
            Camera.onPostRender -= CameraPostRender;
        }

        private void CameraPreCull(Camera currentCamera)
        {
            if (currentCamera == null || underwaterCollider == null || !underwaterCollider.enabled ||
                WeatherMakerFullScreenEffect.GetCameraType(currentCamera) != WeatherMakerCameraType.Normal)
            {
                return;
            }
            Bounds bounds = underwaterCollider.bounds;
            if (bounds.Contains(currentCamera.transform.position))
            {
                if (underwaterCameras.Add(currentCamera))
                {
                    // transition to being underwater
                    Debug.Log("Went underwater: " + currentCamera.name);
                }
                SurfaceScript.GetComponent<MeshRenderer>().enabled = false;//.sharedMaterial.renderQueue = 2501;
                MeshRenderer.enabled = true;
                nullFogZone.enabled = true;
            }
            else
            {
                if (underwaterCameras.Contains(currentCamera))
                {
                    // transition away from being underwater
                    Debug.Log("No longer underwater: " + currentCamera.name);
                    underwaterCameras.Remove(currentCamera);
                }
                SurfaceScript.GetComponent<MeshRenderer>().enabled = true;//.sharedMaterial.renderQueue = 2499;
                MeshRenderer.enabled = false;
                nullFogZone.enabled = false;
            }
        }

        private void CameraPostRender(Camera currentCamera)
        {
            MeshRenderer.enabled = false;
        }
    }
}
