using MapEditorStudio.MapEditor.MapEditTools;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    [DefaultExecutionOrder(-1)]
    public class MapEditorEnvironment : MonoBehaviour
    {
        public static MapEditorEnvironment Instance { get; private set; }

        public ToolSelector ToolSelector;

        public ToolController ToolController;

        public MapEditorPayload Payload = new();

        private void OnEnable()
        {
            Instance = this;
        }
    }
}
