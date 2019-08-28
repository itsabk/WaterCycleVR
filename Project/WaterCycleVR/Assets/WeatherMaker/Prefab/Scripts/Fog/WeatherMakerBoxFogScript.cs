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

using UnityEngine;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    [RequireComponent(typeof(Renderer))]
    public class WeatherMakerBoxFogScript : WeatherMakerFogScript<WeatherMakerFogProfileScript>
    {
        private Renderer fogRenderer;

        protected override void Start()
        {
            base.Start();

            this.fogRenderer = GetComponent<Renderer>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.fogRenderer.sharedMaterial = FogMaterial;
        }

        protected override void OnWillRenderObject()
        {
            base.OnWillRenderObject();

            Bounds b = fogRenderer.bounds;
            float dist = Vector3.Distance(b.min, b.max);
            float radius = dist * 0.5f;
            Vector3 shrinker = b.size * -(1.0f - FogProfile.FogVolumePercentage);
            b.Expand(shrinker);
            Vector4 center = b.center;
            center.w = radius * radius;
            FogMaterial.SetVector("_FogBoxCenter", center);
            FogMaterial.SetVector("_FogBoxMin", b.min);
            FogMaterial.SetVector("_FogBoxMax", b.max);
            FogMaterial.SetVector("_FogBoxMinDir", (b.max - b.min).normalized);
            FogMaterial.SetVector("_FogBoxMaxDir", (b.min - b.max).normalized);
            FogMaterial.SetVector("_FogVolumePower", new Vector4(1.0f / dist, dist * FogProfile.FogEdgeSmoothFactor, dist * FogProfile.FogHeightFalloffPower, FogProfile.FogEdgeFalloffPower));
            fogRenderer.enabled = (FogProfile.FogDensity > 0.0f);
        }
    }
}
