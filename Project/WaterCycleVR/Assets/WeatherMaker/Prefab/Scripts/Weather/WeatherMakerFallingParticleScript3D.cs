﻿//
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
    public class WeatherMakerFallingParticleScript3D : WeatherMakerFallingParticleScript
    {
        [Header("3D Settings")]

        [Tooltip("Whether the particles will anchor themselves to each rendered camera. If false, particles stay where you put them.")]
        public bool IsFirstPerson = true;

        [Tooltip("The height above the camera that the particles will start falling from")]
        public float Height = 25.0f;

        [Tooltip("How far the particle system is ahead of the camera")]
        public float ForwardOffset = -7.0f;

        [Tooltip("The height above the camera that the secondary particles will start falling from")]
        public float SecondaryHeight = 100.0f;

        [Tooltip("How far the secondary particle system is ahead of the camera")]
        public float SecondaryForwardOffset = 25.0f;

        [Tooltip("The top y value of the mist particles")]
        public float MistHeight = 3.0f;

        [Tooltip("Optional animated texture renderer, used to render things like ripples / splashes all inside a shader without need for particle collisions.")]
        public Renderer AnimatedTextureRenderer;

        [Tooltip("Intensity must be greater than this value to show the AnimatedTextureRenderer.")]
        [Range(0.0f, 1.0f)]
        public float AnimatedTextureRendererIntensityThreshold = 0.5f;

        [Tooltip("How much to offset the animated texture renderer should offset from nearest hit below camera. All values should be >= 0. If z is > 0, the drops will rotate with the camera to keep the majority in front of the camera.")]
        public Vector3 AnimatedTextureRendererPositionOffset = new Vector3(0.0f, 0.1f, 0.0f);

        [Tooltip("Layer mask for collision check above player to turn off animated texture if something is hit, i.e. a roof or tree.")]
        public LayerMask AnimatedTextureCollisionMaskAbove = 0;

        [Tooltip("Layer mask for collision check below player to position animated texture, i.e. the ground.")]
        public LayerMask AnimatedTextureCollisionMaskBelow = -1;

        [Header("Particle System Emitters")]
        [Tooltip("ParticleSystem Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemNearWidth = 5.0f;

        [Tooltip("ParticleSystem Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemFarWidth = 70.0f;

        [Tooltip("ParticleSystem Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemNearDepth = 0.25f;

        [Tooltip("ParticleSystem Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemFarDepth = 50.0f;

        [Tooltip("ParticleSystemSecondary Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemSecondaryNearWidth = 5.0f;

        [Tooltip("ParticleSystemSecondary Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemSecondaryFarWidth = 500.0f;

        [Tooltip("ParticleSystemSecondary Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemSecondaryNearDepth = 0.25f;

        [Tooltip("ParticleSystemSecondary Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemSecondaryFarDepth = 50.0f;

        [Tooltip("ParticleSystemMist Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemMistNearWidth = 5.0f;

        [Tooltip("ParticleSystemMist Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemMistFarWidth = 70.0f;

        [Tooltip("ParticleSystemMist Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemMistNearDepth = 0.25f;

        [Tooltip("ParticleSystemMist Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemMistFarDepth = 50.0f;

        private Camera lastCamera;

        private void UpdateCollisionForParticleSystem(ParticleSystem p)
        {
            if (p != null)
            {
                var c = p.collision;
                var s = p.subEmitters;
                c.enabled = CollisionEnabled;
                s.enabled = CollisionEnabled;
            }
        }

        private void CreateMeshEmitter(ParticleSystem p, float nearWidth, float farWidth, float nearDepth, float farDepth)
        {
            if (p == null || p.shape.shapeType != ParticleSystemShapeType.Mesh)
            {
                return;
            }

            Mesh emitter = new Mesh { name = "WeatherMakerFaillingParticleScript3D_Triangle" };
            emitter.vertices = new Vector3[]
            {
                new Vector3(-nearWidth, 0.0f, nearDepth),
                new Vector3(nearWidth, 0.0f, nearDepth),
                new Vector3(-farWidth, 0.0f, farDepth),
                new Vector3(farWidth, 0.0f, farDepth)
            };
            emitter.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            var s = p.shape;
            s.mesh = emitter;
            s.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        }

        private void TransformParticleSystem(ParticleSystem particles, Transform t, float forward, float height, float rotationYModifier)
        {
            if (particles == null || !IsFirstPerson)
            {
                return;
            }
            Vector3 pos = t.position;
            Vector3 anchorForward = t.forward;
            pos.x += anchorForward.x * forward;
            pos.y += height;
            pos.z += anchorForward.z * forward;
            particles.transform.position = pos;
            if (particles.shape.mesh != null)
            {
                Vector3 angles = particles.transform.rotation.eulerAngles;
                particles.transform.rotation = Quaternion.Euler(angles.x, t.rotation.eulerAngles.y * rotationYModifier, angles.z);
            }
        }

        private void PositionAnimatedTexture(Transform anchor)
        {
            if (AnimatedTextureRenderer == null)
            {
                return;
            }
            else if (AnimatedTextureRendererIntensityThreshold >= 1.0f)
            {
                AnimatedTextureRenderer.enabled = false;
            }
            else
            {
                RaycastHit hit;
                Vector3 pos = anchor.position;
                const float duration = 2.0f;

                // check if something above camera, if so disable animated texture
                if (Physics.Raycast(pos, Vector3.up, out hit, 200.0f, AnimatedTextureCollisionMaskAbove, QueryTriggerInteraction.Ignore) &&
                    hit.point.y > pos.y)
                {
                    if (AnimatedTextureRenderer.enabled && AnimatedTextureRenderer.sharedMaterial.GetFloat("_AlphaMultiplierAnimation2") >= 1.0f)
                    {
                        // fade out
                        float start = AnimatedTextureRenderer.sharedMaterial.GetFloat("_AlphaMultiplierAnimation2");
                        TweenFactory.Tween("WeatherMakerPrecipitationAnimatedTextureChange_" + gameObject.GetInstanceID(), start, 0.0f, duration, TweenScaleFunctions.Linear, (t) =>
                        {
                            // Debug.LogFormat("Tween key: {0}, value: {1}, prog: {2}", t.Key, t.CurrentValue, t.CurrentProgress);
                            AnimatedTextureRenderer.sharedMaterial.SetFloat("_AlphaMultiplierAnimation2", t.CurrentValue);
                        },
                        (t) =>
                        {
                            AnimatedTextureRenderer.enabled = false;
                        });
                    }
                }
                else
                {
                    if (AnimatedTextureRendererPositionOffset.z > 0.0f)
                    {
                        AnimatedTextureRenderer.transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up);
                    }
                    if (Physics.Raycast(pos, Vector3.down, out hit, 200.0f, AnimatedTextureCollisionMaskBelow, QueryTriggerInteraction.Ignore))
                    {
                        Vector3 newPos = hit.point;
                        float y = newPos.y;
                        newPos += (anchor.transform.forward * AnimatedTextureRendererPositionOffset.z);
                        newPos.y = y + AnimatedTextureRendererPositionOffset.y;
                        AnimatedTextureRenderer.transform.position = newPos;
                    }
                    AnimatedTextureRenderer.sharedMaterial.SetFloat("_AlphaMultiplierAnimation", Mathf.Lerp(0.0f, 1.0f, Mathf.Pow((Intensity * ExternalIntensityMultiplier) / Mathf.Max(0.01f, AnimatedTextureRendererIntensityThreshold), 3.0f)));
                    if (!AnimatedTextureRenderer.sharedMaterial.IsKeywordEnabled("MAX_DEPTH"))
                    {
                        AnimatedTextureRenderer.sharedMaterial.EnableKeyword("MAX_DEPTH");
                    }
                    AnimatedTextureRenderer.sharedMaterial.SetFloat("_MaxDepth", 32.0f);
                    if (!AnimatedTextureRenderer.enabled)
                    {
                        AnimatedTextureRenderer.enabled = true;
                        float start = AnimatedTextureRenderer.sharedMaterial.GetFloat("_AlphaMultiplierAnimation2");

                        // fade in
                        TweenFactory.Tween("WeatherMakerPrecipitationAnimatedTextureChange_" + gameObject.GetInstanceID(), start, 1.0f, duration, TweenScaleFunctions.Linear, (t) =>
                        {
                            // Debug.LogFormat("Tween key: {0}, value: {1}, prog: {2}", t.Key, t.CurrentValue, t.CurrentProgress);
                            AnimatedTextureRenderer.sharedMaterial.SetFloat("_AlphaMultiplierAnimation2", t.CurrentValue);
                        });
                    }
                }
            }
        }

        private void PositionAllElements(Transform t)
        {
            TransformParticleSystem(ParticleSystem, t, ForwardOffset, Height, 1.0f);
            TransformParticleSystem(ParticleSystemSecondary, t, SecondaryForwardOffset, SecondaryHeight, 1.0f);
            TransformParticleSystem(MistParticleSystem, t, 0.0f, MistHeight, 0.0f);
            PositionAnimatedTexture(t);

        }

        protected override void OnCollisionEnabledChanged()
        {
            base.OnCollisionEnabledChanged();
            UpdateCollisionForParticleSystem(ParticleSystem);
            UpdateCollisionForParticleSystem(ParticleSystemSecondary);
            UpdateCollisionForParticleSystem(MistParticleSystem);
            UpdateCollisionForParticleSystem(ExplosionParticleSystem);
        }

        protected override void Awake()
        {
            base.Awake();

            CreateMeshEmitter(ParticleSystem, ParticleSystemNearWidth, ParticleSystemFarWidth, ParticleSystemNearDepth, ParticleSystemFarDepth);
            CreateMeshEmitter(ParticleSystemSecondary, ParticleSystemSecondaryNearWidth, ParticleSystemSecondaryFarWidth, ParticleSystemSecondaryNearDepth, ParticleSystemSecondaryFarDepth);
            CreateMeshEmitter(MistParticleSystem, ParticleSystemMistNearDepth, ParticleSystemMistFarDepth, ParticleSystemMistNearDepth, ParticleSystemMistFarDepth);
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR

            if (Application.isPlaying)

#endif

            if (AnimatedTextureRenderer != null && AnimatedTextureRenderer.sharedMaterial != null)
            {
                AnimatedTextureRenderer.sharedMaterial = new Material(AnimatedTextureRenderer.sharedMaterial);
                AnimatedTextureRenderer.sharedMaterial.SetFloat("_AlphaMultiplierAnimation2", 0.0f);
            }
        }

        protected override void LateUpdate()
        {
            if (!IsFirstPerson)
            {
                PositionAllElements(transform);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

#if UNITY_EDITOR

            if (AnimatedTextureRenderer != null && AnimatedTextureRenderer.sharedMaterial != null)
            {
                GameObject.Destroy(AnimatedTextureRenderer.sharedMaterial);
            }

#endif

        }

        public override void PreCullCamera(Camera camera)
        {
            if (!IsFirstPerson)
            {
                return;
            }

#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                return;
            }

#endif

            bool isReflection = (WeatherMakerFullScreenEffect.GetCameraType(camera) == WeatherMakerCameraType.Reflection);
            if (!isReflection && camera.cameraType == CameraType.Game)
            {
                PositionAllElements(camera.transform);

                // if we have a world space particle system, simulate it in front of the new camera (i.e. snow), moving the camera multiple
                //  times per frame will not emit particles each time it is moved unfortunately
                if (Intensity > 0.0f && (lastCamera == null || lastCamera != camera) && ParticleSystem.isPlaying &&
                    ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
                {
                    ParticleSystem.Emit((int)Mathf.Round(ParticleSystem.emission.rateOverTime.constant * Time.deltaTime));
                }
                lastCamera = camera;
            }
        }
    }
}