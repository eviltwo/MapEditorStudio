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

        public readonly MapEditorPayload Payload = new();

        public readonly MapEditActionManager ActionManager = new();

        private void OnEnable()
        {
            Instance = this;
        }
    }
}
