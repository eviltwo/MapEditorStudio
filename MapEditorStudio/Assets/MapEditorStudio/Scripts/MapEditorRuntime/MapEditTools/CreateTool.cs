using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditorStudio.MapEditor.MapEditTools
{
    public class CreateTool : MonoBehaviour, IMapEditTool
    {
        public Transform Camera;

        public LayerMask RaycastMask = ~0;

        public float RaycastDistance = 10f;

        public PlayerInput PlayerInput;

        public InputActionReference PrimaryAction;

        private float _angle;

        public float SnapAngle = 45f;

        public InputActionReference SecondaryAction;

        private bool _isSecondaryActionDown;

        public float SmoothAngleTriggerDistance = 100f;

        private Vector2 _moveDistanceForSmoothAngleTrigger;

        private bool _isSmoothAngle;

        public float SmoothAngleMoveSensitivity = 1f;

        public InputActionReference SmoothAngleMoveAction;

        public bool DisableCameraLookOnSmoothAngle = true;

        public InputActionReference CameraLookAction;

        private readonly MapAssetPreview _mapAssetPreview = new();

        private void Start()
        {
            var toolController = MapEditorEnvironment.Instance.ToolController;
            if (toolController != null)
            {
                toolController.RegisterTool(ToolTypes.Create, this);
            }
        }

        public void Activate()
        {
            enabled = true;
        }

        public void Deactivate()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            if (PlayerInput != null)
            {
                PlayerInput.onActionTriggered += OnInputActionTriggered;
            }
        }

        private void OnDisable()
        {
            _mapAssetPreview.Clear();
            if (PlayerInput != null)
            {
                PlayerInput.onActionTriggered -= OnInputActionTriggered;
            }
        }

        private void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            // Create object
            if (PrimaryAction != null && context.action.id == PrimaryAction.action.id && context.started)
            {
                if (Physics.Raycast(Camera.position, Camera.forward, out var hit, RaycastDistance, RaycastMask))
                {
                    var selectedMapAsset = MapEditorEnvironment.Instance.Payload.SelectedAsset;
                    if (selectedMapAsset == null) return;
                    var actionManager = MapEditorEnvironment.Instance.ActionManager;
                    if (actionManager != null)
                    {
                        actionManager.ExecuteAction(new CreateAction(selectedMapAsset.Asset, hit.point, CalculateRotation(), Vector3.one));
                    }
                }
            }

            // Rotate angle with snap
            if (SecondaryAction != null && context.action.id == SecondaryAction.action.id)
            {
                _isSecondaryActionDown = context.started || context.performed;
                if (context.started)
                {
                    _isSmoothAngle = false;
                    _moveDistanceForSmoothAngleTrigger = Vector2.zero;
                }
                else if (context.canceled)
                {
                    if (!_isSmoothAngle)
                    {
                        _angle += SnapAngle;
                        _mapAssetPreview.SetRotation(CalculateRotation());
                    }

                    _isSmoothAngle = false;
                }
            }

            // Rotate angle smoothly
            if (_isSecondaryActionDown && SmoothAngleMoveAction != null && context.action.id == SmoothAngleMoveAction.action.id && context.performed)
            {
                var inputValue = context.ReadValue<Vector2>();
                _moveDistanceForSmoothAngleTrigger += inputValue;
                if (_moveDistanceForSmoothAngleTrigger.sqrMagnitude >= SmoothAngleTriggerDistance * SmoothAngleTriggerDistance)
                {
                    _isSmoothAngle = true;
                }

                if (_isSmoothAngle)
                {
                    _angle += inputValue.x * SmoothAngleMoveSensitivity;
                    _mapAssetPreview.SetRotation(CalculateRotation());
                }
            }

            // Switch camera look action active state
            if (DisableCameraLookOnSmoothAngle && CameraLookAction != null)
            {
                var shouldLock = _isSecondaryActionDown;
                var currentActive = CameraLookAction.action.enabled;
                var shouldActive = !shouldLock;
                if (currentActive && !shouldActive)
                {
                    CameraLookAction.action.Disable();
                }
                else if (!currentActive && shouldActive)
                {
                    CameraLookAction.action.Enable();
                }
            }
        }

        private Quaternion CalculateRotation()
        {
            return Quaternion.AngleAxis(_angle, Vector3.up);
        }

        private void Update()
        {
            var selectedMapAsset = MapEditorEnvironment.Instance.Payload.SelectedAsset;
            if (selectedMapAsset == null)
            {
                _mapAssetPreview.Clear();
                return;
            }

            _mapAssetPreview.SetMapAsset(selectedMapAsset.GUID, selectedMapAsset.Asset);
        }

        private void LateUpdate()
        {
            if (Physics.Raycast(Camera.position, Camera.forward, out var hit, RaycastDistance, RaycastMask))
            {
                _mapAssetPreview.SetPosition(hit.point);
            }
        }

        private class MapAssetPreview
        {
            private string _mapAssetGuid;

            private GameObject _mapObject;

            private List<Collider> _colliderBuffer = new();

            public void SetMapAsset(string guid, GameObject mapPrefab)
            {
                if (_mapAssetGuid == guid) return;
                Clear();
                _mapAssetGuid = guid;
                _mapObject = Instantiate(mapPrefab);
                _mapObject.GetComponentsInChildren(_colliderBuffer);
                foreach (var collider in _colliderBuffer)
                {
                    collider.enabled = false;
                }
            }

            public void Clear()
            {
                _mapAssetGuid = null;
                if (_mapObject != null)
                {
                    Destroy(_mapObject);
                    _mapObject = null;
                }
            }

            public void SetPosition(Vector3 position)
            {
                if (_mapObject == null) return;
                _mapObject.transform.position = position;
            }

            public void SetRotation(Quaternion rotation)
            {
                if (_mapObject == null) return;
                _mapObject.transform.rotation = rotation;
            }

            public Quaternion GetRotation() => _mapObject.transform.rotation;
        }
    }
}
