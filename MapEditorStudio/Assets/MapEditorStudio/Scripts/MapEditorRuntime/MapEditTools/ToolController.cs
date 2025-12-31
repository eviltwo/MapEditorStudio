using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor.MapEditTools
{
    public interface IMapEditTool
    {
        void Activate();
        void Deactivate();
    }

    public class ToolController : MonoBehaviour
    {
        private readonly Dictionary<ToolTypes, IMapEditTool> _tools = new();

        private IMapEditTool _lastActiveTool;

        public bool SharedActive { get; private set; }

        private void OnEnable()
        {
            var toolSelector = MapEditorEnvironment.Instance.ToolSelector;
            if (toolSelector != null)
            {
                toolSelector.OnChangeSelectedTool += OnChangeSelectedTool;
            }
        }

        private void OnDisable()
        {
            var toolSelector = MapEditorEnvironment.Instance.ToolSelector;
            if (toolSelector != null)
            {
                toolSelector.OnChangeSelectedTool -= OnChangeSelectedTool;
            }
        }

        public void RegisterTool(ToolTypes toolType, IMapEditTool tool)
        {
            _tools[toolType] = tool;
            var toolSelector = MapEditorEnvironment.Instance.ToolSelector;
            if (toolSelector != null && toolSelector.SelectedTool == toolType)
            {
                _lastActiveTool?.Deactivate();
                tool.Activate();
                _lastActiveTool = tool;
            }
            else
            {
                tool.Deactivate();
            }
        }

        public void UnregisterTool(ToolTypes toolType)
        {
            _tools.Remove(toolType);
        }

        public void SetSharedActive(bool active)
        {
            SharedActive = active;
            if (SharedActive)
            {
                _lastActiveTool?.Activate();
            }
            else
            {
                _lastActiveTool?.Deactivate();
            }
        }

        private void OnChangeSelectedTool(ToolTypes toolType)
        {
            _lastActiveTool?.Deactivate();
            if (_tools.TryGetValue(toolType, out var tool) && tool != null)
            {
                tool.Activate();
                _lastActiveTool = tool;
            }
        }
    }
}
