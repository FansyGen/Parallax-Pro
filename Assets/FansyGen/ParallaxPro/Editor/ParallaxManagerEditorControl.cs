using FansyGen.ParallaxPro;
using UnityEditor;
using UnityEngine;

namespace FansyGen
{
    [CustomEditor(typeof(ParallaxManager))]
    public class ParallaxManagerEditorControl : Editor
    {
        public ParallaxData parallaxData;

        private SerializedProperty targetProp;
        private SerializedProperty parallaxDirectionProp;
        private SerializedProperty layerObjectsProp;
        private SerializedProperty interactableProp;
        private SerializedProperty layerSpeedsProp;

        private SerializedProperty timingFunctionProp;
        private SerializedProperty transitionDurationProp;
        private SerializedProperty stepsProp;
        private SerializedProperty bezierCurveProp;

        private void OnEnable()
        {
            targetProp = serializedObject.FindProperty("target");
            parallaxDirectionProp = serializedObject.FindProperty("parallaxDirection");
            layerObjectsProp = serializedObject.FindProperty("layerObjects");
            interactableProp = serializedObject.FindProperty("calculateSpeedAutomatically");
            layerSpeedsProp = serializedObject.FindProperty("layerSpeeds");

            timingFunctionProp = serializedObject.FindProperty("timingFunction");
            transitionDurationProp = serializedObject.FindProperty("transitionDuration");
            stepsProp = serializedObject.FindProperty("steps");
            bezierCurveProp = serializedObject.FindProperty("bezierCurve");



            if (parallaxData == null)
            {
                parallaxData = ParallaxData.instance;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.normal.background = MakeTexture(1, 1, Color.white);

            EditorGUILayout.BeginVertical(headerStyle);
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(parallaxData.icon, GUILayout.Width(50), GUILayout.Height(50));

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.font = parallaxData.scriptNameFont;
            labelStyle.normal.textColor = Color.black;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 20;
            labelStyle.fixedHeight = 50;
            labelStyle.fixedWidth = 1000;

            EditorGUILayout.LabelField(parallaxData.scriptName, labelStyle);

            GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
            versionStyle.normal.textColor = Color.grey;
            versionStyle.alignment = TextAnchor.UpperRight;
            versionStyle.fontStyle = FontStyle.Normal;
            versionStyle.fontSize = 10;
            versionStyle.fixedHeight = 50;
            versionStyle.fixedWidth = 80;

            EditorGUILayout.LabelField("V: " + parallaxData.version, versionStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            // Display the default inspector GUI elements
            EditorGUILayout.PropertyField(targetProp);
            EditorGUILayout.PropertyField(parallaxDirectionProp);
            EditorGUILayout.PropertyField(layerObjectsProp);

            // Allow controlling interactability of layerSpeeds list
            EditorGUILayout.PropertyField(interactableProp);

            if (!interactableProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(layerSpeedsProp, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(timingFunctionProp);
            EditorGUILayout.PropertyField(transitionDurationProp);

            if (timingFunctionProp.enumValueIndex == (int)TransitionTimingFunction.Steps)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(stepsProp, true);
                EditorGUI.indentLevel--;
            }

            if (timingFunctionProp.enumValueIndex == (int)TransitionTimingFunction.CubicBezier)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(bezierCurveProp, true);
                EditorGUI.indentLevel--;
            }


            serializedObject.ApplyModifiedProperties();
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}