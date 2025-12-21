using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor.UI
{
    public class ToolIconController : MonoBehaviour
    {
        [SerializeField]
        private ToolIconUI SourceUI;

        [SerializeField]
        private Transform ContentParent;

        [System.Serializable]
        private class ToolIconMap
        {
            public ToolTypes ToolType;
            public Sprite Icon;
        }

        [SerializeField]
        private ToolIconMap[] ToolIconMaps;

        private readonly Dictionary<ToolTypes, ToolIconUI> _createdIcons = new();

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

        private void Start()
        {
            var selectedTool = ToolTypes.Create;
            var toolSelector = MapEditorEnvironment.Instance.ToolSelector;
            if (toolSelector != null)
            {
                selectedTool = toolSelector.SelectedTool;
            }

            foreach (var map in ToolIconMaps)
            {
                var content = Instantiate(SourceUI, ContentParent);
                content.gameObject.SetActive(true);
                content.SetIcon(map.Icon);
                content.SetSelected(map.ToolType == selectedTool);
                _createdIcons.Add(map.ToolType, content);
            }

            SourceUI.gameObject.SetActive(false);
        }

        private void OnChangeSelectedTool(ToolTypes tool)
        {
            foreach (var icon in _createdIcons)
            {
                icon.Value.SetSelected(icon.Key == tool);
            }
        }
    }
}
