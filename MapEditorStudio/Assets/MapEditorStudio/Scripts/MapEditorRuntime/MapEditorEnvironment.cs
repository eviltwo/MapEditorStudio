using MapEditorStudio.MapEditor.MapEditTools;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapEditorEnvironment : MonoBehaviour
    {
        public static MapEditorEnvironment Instance { get; private set; }

        public FirstPersonMapEditTool MapEditTool;

        public MapEditorPayload Payload = new();

        private void Awake()
        {
            Instance = this;
        }
    }
}
