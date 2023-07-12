using UnityEditor;
using UnityEngine;

namespace FansyGen.ParallaxPro
{
    ///[CreateAssetMenu(fileName = "ParallaxData", menuName = "Custom/ParallaxData")]
    public class ParallaxData : ScriptableObject
    {
        public static ParallaxData instance;

        public Texture2D icon;
       [HideInInspector] public string scriptName = "Parallax Manager";
        public Font scriptNameFont;
       [HideInInspector] [Range(0, 10)] public float version = 0.1f;


        public static ParallaxData Instance
        {
            get
            {
                if (instance == null)
                {
                    // If instance is null, try to find an existing instance in the project
                    instance = Resources.Load<ParallaxData>("ParallaxData");

                    if (instance == null)
                    {
                        // If no instance exists, create a new one
                        instance = CreateInstance<ParallaxData>();
                        instance.name = "ParallaxData";

                        // Save the instance as an asset in the project
                        string assetPath = "Assets/MySingletonScriptableObject.asset";
                        UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                    }
                }

                return instance;
            }
        }
    }
}