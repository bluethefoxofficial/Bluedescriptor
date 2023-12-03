using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = System.Drawing.Color;

namespace Bluedescriptor_Rewritten.Classes
{
    public class AdvancedCameraRaycastCulling : MonoBehaviour
    {
        public Camera mainCamera;
        public float cullingDistance = 50f;
        public LayerMask cullingLayerMask;
        public int raysPerAxis = 5;
        Vector3 lastCameraPosition;
        Quaternion lastCameraRotation;
        List<Renderer> cullableRenderers = new List<Renderer>();

        public AdvancedCameraRaycastCulling(IntPtr ptr)
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            if (mainCamera != null)
            {
                MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
                enabled = false;
            }
            else
            {
                InitializeCullableObjects();
                MelonLogger.Msg(Color.MediumPurple, "camera found");
            }
        }

        void InitializeCullableObjects()
        {
            if (cullableRenderers == null)
                cullableRenderers = new List<Renderer>();

            var layer = LayerMask.NameToLayer("PlayerNetwork");

            foreach (Renderer renderer in FindObjectsOfType<Renderer>())
            {
                if (!renderer && renderer.gameObject.layer != layer)
                {
                    var scene1 = renderer.transform.root.gameObject.scene;
                    int num;

                    if (scene1.name != null)
                    {
                        var scene2 = renderer.transform.root.gameObject.scene;
                        num = scene2.name == "DontGameObject.DestroyOnLoad" ? 1 : 0;
                    }
                    else
                        num = 1;

                    if (num == 0)
                        cullableRenderers.Add(renderer);
                }
            }
        }

        public void reset() => cullableRenderers.Clear();

        public void rescancam()
        {
            try
            {
                mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

                if (mainCamera != null)
                {
                    MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
                    enabled = false;
                }
                else
                {
                    InitializeCullableObjects();
                    MelonLogger.Msg(Color.MediumPurple, "camera found");
                }
            }
            catch
            {
            }
        }

        public IEnumerator Start()
        {
            mainCamera = Camera.main;

            if (mainCamera != null)
            {
                MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
            }
            else
            {
                lastCameraPosition = mainCamera.transform.position;
                lastCameraRotation = mainCamera.transform.rotation;
                yield return StartCoroutine(CullObjects());
                MelonLogger.Log("Cull started");
            }
        }

        public IEnumerator CheckForCameraMovement()
        {
            while (enabled)
            {
                if (mainCamera.transform.position != lastCameraPosition || mainCamera.transform.rotation != lastCameraRotation)
                {
                    yield return StartCoroutine(CullObjects());
                    lastCameraPosition = mainCamera.transform.position;
                    lastCameraRotation = mainCamera.transform.rotation;
                }

                yield return null;
            }
        }

        IEnumerator CullObjects()
        {
            foreach (Renderer cullableRenderer in cullableRenderers)
            {
                var renderer = cullableRenderer;

                if (renderer != null)
                {
                    var withinCullingDistance = (double)Vector3.Distance(mainCamera.transform.position, renderer.transform.position) <= cullingDistance;
                    var isVisible = withinCullingDistance && IsObjectVisible(renderer);
                    renderer.enabled = isVisible;
                    renderer = null;
                }
            }

            yield return null;
        }

        bool IsObjectVisible(Renderer renderer)
        {
            if (!renderer) // Check if the renderer is null
                return false;

            var mainCamera = this.mainCamera;
            var position = mainCamera.transform.position;

            var bounds = renderer.bounds;
            var center = bounds.center;

            if (mainCamera.WorldToViewportPoint(center).z < 0.0f)
                return false;

            for (int index1 = 0; index1 < raysPerAxis; ++index1)
            {
                for (int index2 = 0; index2 < raysPerAxis; ++index2)
                {
                    var vector3 = new Vector3((float)index1 / (raysPerAxis - 1), (float)index2 / (raysPerAxis - 1), 0.0f);

                    RaycastHit raycastHit;

                    if (Physics.Raycast(mainCamera.ViewportPointToRay(vector3), out raycastHit, cullingDistance, LayerMask.GetMask("YourLayerName")) && raycastHit.transform == renderer.transform)
                        return true;
                }
            }

            return false;
        }
    }
}