using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditorStudio.MapEditor
{
    public class ToolSelector : MonoBehaviour
    {
        public PlayerInput PlayerInput;

        public InputActionReference ScrollAction;

        public float TriggerThreshold = 0.5f;

        public float TriggerInterval = 0.2f;

        public ToolTypes SelectedTool { get; private set; } = ToolTypes.Create;

        public event System.Action<ToolTypes> OnChangeSelectedTool;

        private float _stackedScrollValue;

        private float _scrollElapsedTime;

        private void OnEnable()
        {
            if (PlayerInput == null) return;
            PlayerInput.onActionTriggered += OnInputActionTriggered;
        }

        private void OnDisable()
        {
            if (PlayerInput == null) return;
            PlayerInput.onActionTriggered -= OnInputActionTriggered;
        }

        private void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            if (ScrollAction != null
                && context.action.id == ScrollAction.action.id
                && context.performed
                && _scrollElapsedTime >= TriggerInterval)
            {
                _stackedScrollValue += context.ReadValue<float>();
                if (Mathf.Abs(_stackedScrollValue) >= TriggerThreshold)
                {
                    _stackedScrollValue = 0;
                    _scrollElapsedTime = 0;
                    if (_stackedScrollValue > 0)
                    {
                        SelectNext();
                    }
                    else
                    {
                        SelectPrevious();
                    }
                }
            }
        }

        private void Update()
        {
            _scrollElapsedTime += Time.deltaTime;
        }

        public void Select(ToolTypes tool)
        {
            if (tool == SelectedTool) return;
            SelectedTool = tool;
            OnChangeSelectedTool?.Invoke(tool);
            Debug.Log($"Selected Tool : {tool}");
        }

        public void SelectNext() => Select(ToolTypesUtility.GetNext(SelectedTool));

        public void SelectPrevious() => Select(ToolTypesUtility.GetPrevious(SelectedTool));
    }
}
