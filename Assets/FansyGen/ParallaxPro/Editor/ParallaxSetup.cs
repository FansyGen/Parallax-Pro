using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FansyGen.ParallaxPro.Editor
{
    public class ParallaxSetup : EditorWindow
    {
        private List<GameObject> prefabList = new List<GameObject>();
        private ParallaxDirection parallaxDirection;
        private Transform target;

        private Vector2 scrollPosition;

        [MenuItem("Tools/Parallax Setup")]
        public static void ShowWindow()
        {
            GetWindow<ParallaxSetup>("Parallax Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Parallax Background Setup", EditorStyles.boldLabel);

            GUILayout.Space(10f);

            GUILayout.Label("Parallax Settings", EditorStyles.boldLabel);
            parallaxDirection = (ParallaxDirection)EditorGUILayout.EnumPopup("Parallax Direction", parallaxDirection);
            target = (Transform)EditorGUILayout.ObjectField("Target", target, typeof(Transform), true);

            GUILayout.Space(10f);

            GUILayout.Label("Prefab List", EditorStyles.boldLabel);
            ShowPrefabList();

            GUILayout.Space(10f);

            if (GUILayout.Button("Setup Parallax Background"))
            {
                SetupParallaxBackground();
            }
        }

        private void ShowPrefabList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < prefabList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                prefabList[i] = (GameObject)EditorGUILayout.ObjectField("Prefab " + (i + 1), prefabList[i], typeof(GameObject), true);
                if (GUILayout.Button("Remove", GUILayout.Width(80f)))
                {
                    prefabList.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Prefab", GUILayout.Width(100f)))
            {
                prefabList.Add(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void SetupParallaxBackground()
        {
            if (prefabList.Count == 0)
            {
                Debug.LogWarning("Prefab list is empty! Please add prefabs.");
                return;
            }

            if (target == null)
            {
                Debug.LogWarning("Target not assigned! Please assign a target object.");
                return;
            }

            GameObject parallaxBackground = new GameObject("ParallaxBackground");
            ParallaxManager parallaxScript = parallaxBackground.AddComponent<ParallaxManager>();
            parallaxScript.target = target;
            parallaxScript.parallaxDirection = parallaxDirection;

            // Get the reference to the main camera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found! Please ensure there is a camera in the scene.");
                return;
            }

            for (int i = 0; i < prefabList.Count; i++)
            {
                if (prefabList[i] == null)
                {
                    Debug.LogWarning("Prefab at index " + i + " is null! Skipping...");
                    continue;
                }

                GameObject layerObject = Instantiate(prefabList[i], parallaxBackground.transform);
                layerObject.name = "Layer_" + i;
                layerObject.transform.localPosition = Vector3.zero;

                SpriteRenderer spriteRenderer = layerObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = i + -prefabList.Count;

                    // Calculate the scale based on the sprite size and Unity resolution
                    float worldScreenHeight = mainCamera.orthographicSize * 2f;
                    float worldScreenWidth = worldScreenHeight * mainCamera.aspect;

                    Vector2 spriteSize = spriteRenderer.bounds.size;
                    Vector2 scale = new Vector2(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y);

                    layerObject.transform.localScale = scale;
                }

                if (i > 0)
                {
                    for (int c = 0; c < 2; c++)
                    {
                        GameObject childLayer = Instantiate(prefabList[i], layerObject.transform);
                        childLayer.name = "Layer_" + c;
                        childLayer.transform.SetParent(layerObject.transform);

                        if (parallaxDirection == ParallaxDirection.Horizontal)
                        {
                            if (c == 0)
                            {
                                float offsetX = layerObject.transform.position.x + (spriteRenderer.size.x);
                                childLayer.transform.localPosition = new Vector3(offsetX, 0, 0);
                            }
                            else
                            {
                                float offsetX = layerObject.transform.position.x + (spriteRenderer.size.x);
                                childLayer.transform.localPosition = new Vector3(-offsetX, 0, 0);

                            }
                        }
                        else
                        {
                            if (c == 0)
                            {
                                float offsetX = layerObject.transform.position.y + (spriteRenderer.size.y);
                                childLayer.transform.localPosition = new Vector3(0, offsetX, 0);
                            }
                            else
                            {
                                float offsetX = layerObject.transform.position.y + (spriteRenderer.size.y);
                                childLayer.transform.localPosition = new Vector3(0, -offsetX, 0);

                            }
                        }

                        SpriteRenderer childRenderer = childLayer.GetComponent<SpriteRenderer>();
                        if (childRenderer != null)
                        {
                            childRenderer.sortingOrder = spriteRenderer.sortingOrder;
                        }
                    }
                }

                parallaxScript.layerObjects.Add(spriteRenderer.gameObject);
            }

            Debug.Log("Parallax Background setup completed!");
        }

    }
}