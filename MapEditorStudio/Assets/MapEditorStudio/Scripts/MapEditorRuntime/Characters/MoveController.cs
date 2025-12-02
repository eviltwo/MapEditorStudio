using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditorStudio.MapEditor.Characters
{
    public class MoveController : MonoBehaviour
    {
        public CharacterController CharacterController;

        public PlayerInput PlayerInput;

        public InputActionReference MoveAction;

        public float MoveSpeed = 10f;

        public float Friction = 1f;

        private Vector3 _velocity;
        private Vector2 _moveInput;

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
            if (MoveAction != null && context.action.id == MoveAction.action.id)
            {
                _moveInput = context.ReadValue<Vector2>();
            }
        }

        private void Update()
        {
            if (CharacterController == null) return;

            _velocity += Physics.gravity * Time.deltaTime;
            if (CharacterController.isGrounded)
            {
                _velocity.y = Mathf.Min(_velocity.y, 0);

                var f = Mathf.Clamp01(Friction * Time.deltaTime);
                _velocity.x -= _velocity.x * f;
                _velocity.z -= _velocity.z * f;
            }

            var moveInputXZ = new Vector3(_moveInput.x, 0, _moveInput.y);
            _velocity += moveInputXZ * (MoveSpeed * Time.deltaTime);
            CharacterController.Move(_velocity * Time.deltaTime);
        }
    }
}
