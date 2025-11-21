using UnityEditor;
using UnityEngine;

namespace MapEditorStudio.Editor
{
    [CustomEditor(typeof(FolderMapAssetList))]
    public class FolderMapAssetListEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var folderGUIDProperty = serializedObject.FindProperty(nameof(FolderMapAssetList.FolderGUID));
            var folderPath = AssetDatabase.GUIDToAssetPath(folderGUIDProperty.stringValue);
            var folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
            folderAsset = EditorGUILayout.ObjectField(folderAsset, typeof(DefaultAsset), false) as DefaultAsset;
            if (folderAsset == null)
            {
                folderGUIDProperty.stringValue = string.Empty;
            }
            else
            {
                folderPath = AssetDatabase.GetAssetPath(folderAsset);
                folderGUIDProperty.stringValue = AssetDatabase.AssetPathToGUID(folderPath);
            }

            var tagsProperty = serializedObject.FindProperty(nameof(FolderMapAssetList.Tags));
            EditorGUILayout.PropertyField(tagsProperty);

            var itemsProperty = serializedObject.FindProperty(nameof(FolderMapAssetList.Items));
            using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(folderPath)))
            {
                if (GUILayout.Button("Collect"))
                {
                    var guids = AssetDatabase.FindAssets($"t:Prefab", new[] { folderPath });
                    itemsProperty.arraySize = guids.Length;
                    for (int i = 0; i < guids.Length; i++)
                    {
                        var guid = guids[i];
                        var mapAssetData = new MapAssetData
                        {
                            GUID = guid,
                            Asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)),
                            Tags = System.Array.Empty<string>(),
                            DisplayName = string.Empty,
                        };
                        var elementProperty = itemsProperty.GetArrayElementAtIndex(i);
                        ApplyMapAssetProperty(elementProperty, mapAssetData);
                    }
                }
            }

            EditorGUILayout.PropertyField(itemsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private static void ApplyMapAssetProperty(SerializedProperty mapAssetProperty, MapAssetData mapAssetData)
        {
            mapAssetProperty.FindPropertyRelative(nameof(MapAssetData.GUID)).stringValue = mapAssetData.GUID;
            mapAssetProperty.FindPropertyRelative(nameof(MapAssetData.Asset)).objectReferenceValue = mapAssetData.Asset;
            ApplyStringArrayProperty(mapAssetProperty.FindPropertyRelative(nameof(MapAssetData.Tags)), mapAssetData.Tags);
            mapAssetProperty.FindPropertyRelative(nameof(MapAssetData.DisplayName)).stringValue = mapAssetData.DisplayName;
        }

        private static void ApplyStringArrayProperty(SerializedProperty strArrayProperty, string[] strData)
        {
            strArrayProperty.arraySize = strData.Length;
            for (int i = 0; i < strData.Length; i++)
            {
                strArrayProperty.GetArrayElementAtIndex(i).stringValue = strData[i];
            }
        }
    }
}
