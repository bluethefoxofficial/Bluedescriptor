using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public class AdvancedCameraRaycastCulling : MonoBehaviour
    {
        public AdvancedCameraRaycastCulling(IntPtr ptr) : base()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            if (mainCamera == null)
            {
                MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
                enabled = false;
            }
            else
            {
                InitializeCullableObjects();
                MelonLogger.Msg(System.Drawing.Color.MediumPurple, "camera found");
            }
        }

        public Camera mainCamera;
        public float cullingDistance = 50f;
        public LayerMask cullingLayerMask;
        public int raysPerAxis = 5;

        Vector3 lastCameraPosition;
        Quaternion lastCameraRotation;
        List<Renderer> cullableRenderers = new List<Renderer>();

        void InitializeCullableObjects()
        {
            // Ensure the list of cullable renderers has been initialized
            if (cullableRenderers == null)
                cullableRenderers = new List<Renderer>();

            // Define the layer that should not be culled
            var playerNetworkLayer = LayerMask.NameToLayer("PlayerNetwork");

            // Find all renderers in the scene
            var allRenderers = FindObjectsOfType<Renderer>();

            foreach (Renderer renderer in allRenderers)
            {
                if (renderer == null) // Check if the renderer is null
                    continue;

                // Skip objects on the PlayerNetwork layer
                if (renderer.gameObject.layer == playerNetworkLayer)
                    continue;

                // Skip objects in the DontDestroyOnLoad scene
                if (renderer.transform.root.gameObject.scene.name == null || // Check if scene name is null
                    renderer.transform.root.gameObject.scene.name == "DontDestroyOnLoad")
                    continue;

                // Add the renderer to the list of cullable renderers
                cullableRenderers.Add(renderer);
            }
        }

        public void reset() => cullableRenderers.Clear();

        public void rescancam()
        {
            try
            {
                mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

                if (mainCamera == null)
                {
                    MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
                    enabled = false;
                }
                else
                {
                    InitializeCullableObjects();
                    MelonLogger.Msg(System.Drawing.Color.MediumPurple, "camera found");
                }
            }
            catch { }
        }

        public IEnumerator Start()
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                MelonLogger.Error("Main camera not found. Please ensure your main camera is tagged as 'MainCamera'.");
                yield break; // Exit the coroutine if there is no main camera
            }

            lastCameraPosition = mainCamera.transform.position;
            lastCameraRotation = mainCamera.transform.rotation;
            yield return StartCoroutine(CullObjects()); // Start the initial culling as a coroutine

            MelonLogger.Log("Cull started");
        }

        public IEnumerator CheckForCameraMovement()
        {
            while (enabled) // Run as long as this component is enabled
            {
                if (mainCamera.transform.position != lastCameraPosition || mainCamera.transform.rotation != lastCameraRotation)
                {
                    yield return StartCoroutine(CullObjects()); // Start culling as a coroutine
                    lastCameraPosition = mainCamera.transform.position;
                    lastCameraRotation = mainCamera.transform.rotation;
                }

                yield return null; // Wait until the next frame
            }
        }

        IEnumerator CullObjects()
        {
            foreach (Renderer renderer in cullableRenderers)
            {
                if (renderer == null)
                {
                    continue; // Skip if the renderer has been destroyed or is null
                }

                var withinCullingDistance = Vector3.Distance(mainCamera.transform.position, renderer.transform.position) <= cullingDistance;
                var isVisible = withinCullingDistance && IsObjectVisible(renderer);

                renderer.enabled = isVisible;
            }

            yield return null;
        }

        bool IsObjectVisible(Renderer renderer)
        {
            var cameraPosition = mainCamera.transform.position;
            var viewportCenter = mainCamera.WorldToViewportPoint(renderer.bounds.center);

            if (viewportCenter.z < 0) return false; // Object is behind the camera

            for (int x = 0; x < raysPerAxis; x++)
            {
                for (int y = 0; y < raysPerAxis; y++)
                {
                    var point = new Vector3(
                        (float)x / (raysPerAxis - 1),
                        (float)y / (raysPerAxis - 1),
                        0);

                    var ray = mainCamera.ViewportPointToRay(point);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, cullingDistance, cullingLayerMask))
                    {
                        if (hit.transform == renderer.transform)
                        {
                            return true; // Object is visible
                        }
                    }
                }
            }

            return false; // Object is not visible
        }
    }
}