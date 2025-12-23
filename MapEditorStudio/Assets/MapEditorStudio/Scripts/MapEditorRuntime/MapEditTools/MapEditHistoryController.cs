using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditorStudio.MapEditor
{
    public class MapEditHistoryController : MonoBehaviour
    {
        public PlayerInput PlayerInput;

        public InputActionReference UndoAction;

        public InputActionReference RedoAction;

        private void OnEnable()
        {
            if (PlayerInput != null)
            {
                PlayerInput.onActionTriggered += OnInputActionTriggered;
            }
        }

        private void OnDisable()
        {
            if (PlayerInput != null)
            {
                PlayerInput.onActionTriggered -= OnInputActionTriggered;
            }
        }

        private void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            if (UndoAction != null && context.action.id == UndoAction.action.id && context.started)
            {
                var actionManager = MapEditorEnvironment.Instance.ActionManager;
                if (actionManager != null)
                {
                    actionManager.UndoAction();
                }
            }

            if (RedoAction != null && context.action.id == RedoAction.action.id && context.started)
            {
                var actionManager = MapEditorEnvironment.Instance.ActionManager;
                if (actionManager != null)
                {
                    actionManager.RedoAction();
                }
            }
        }
    }
}
