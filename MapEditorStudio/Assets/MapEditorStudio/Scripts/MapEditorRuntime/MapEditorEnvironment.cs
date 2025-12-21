using MapEditorStudio.MapEditor.MapEditTools;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    [DefaultExecutionOrder(-1)]
    public class MapEditorEnvironment : MonoBehaviour
    {
        public static MapEditorEnvironment Instance { get; private set; }

        public FirstPersonMapEditTool MapEditTool;

        public ToolSelector ToolSelector;

        public MapEditorPayload Payload = new();

        private void OnEnable()
        {
            Instance = this;
        }
    }
}
