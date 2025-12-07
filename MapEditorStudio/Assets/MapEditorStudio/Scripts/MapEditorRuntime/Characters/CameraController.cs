using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditorStudio.MapEditor.Characters
{
    public class CameraController : MonoBehaviour
    {
        public Transform Camera;

        public Transform FollowTarget;

        public Vector3 FollowOffset = Vector3.zero;

        public PlayerInput PlayerInput;

        public InputActionReference LookAction;

        public float HorizontalSensitivity = 1f;

        public float VerticalSensitivity = 1f;

        private Vector2 _lookInput;

        private Vector2 _angles = Vector2.zero;

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
            if (LookAction != null && context.action.id == LookAction.action.id)
            {
                _lookInput = context.ReadValue<Vector2>();
            }
        }

        private void Update()
        {
            Camera.position = FollowTarget.position + FollowTarget.rotation * FollowOffset;

            _angles.x -= _lookInput.y * HorizontalSensitivity;
            _angles.x = Mathf.Clamp(_angles.x, -90f, 90f);
            _angles.y += _lookInput.x * VerticalSensitivity;
            _angles.y = _angles.y % 360f;
            Camera.localRotation = Quaternion.Euler(_angles.x, _angles.y, 0);
        }
    }
}
