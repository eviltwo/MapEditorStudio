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

        public bool EnableObjectSnap = true;

        private readonly SelectedMapAssetChangeEventDetector _selectedMapAssetChangeEventDetector = new();

        private ObjectPlacementCalculator _objectPlacementCalculator;

        private readonly MapAssetPreview _mapAssetPreview = new();

        private void Start()
        {
            var toolController = MapEditorEnvironment.Instance.ToolController;
            if (toolController != null)
            {
                toolController.RegisterTool(ToolTypes.Create, this);
            }

            _selectedMapAssetChangeEventDetector.OnSelectedAssetChanged += OnChangeSelectedAsset;
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
            _selectedMapAssetChangeEventDetector.Clear();
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
                        _objectPlacementCalculator.SetSnapPoint(hit.point, hit.normal);
                        _objectPlacementCalculator.Calculate(out var position, out var rotation);
                        actionManager.ExecuteAction(new CreateAction(selectedMapAsset.Asset, position, rotation, Vector3.one));
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
                        var angle = ObjectSnapUtility.GetNearestSnappedAngle(_objectPlacementCalculator.Angle, SnapAngle);
                        _objectPlacementCalculator.SetAngle(angle + SnapAngle);
                        _objectPlacementCalculator.Calculate(out var position, out var rotation);
                        _mapAssetPreview.SetPosition(position);
                        _mapAssetPreview.SetRotation(rotation);
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
                    _objectPlacementCalculator.SetAngle(_objectPlacementCalculator.Angle + inputValue.x * SmoothAngleMoveSensitivity);
                    _objectPlacementCalculator.Calculate(out var position, out var rotation);
                    _mapAssetPreview.SetPosition(position);
                    _mapAssetPreview.SetRotation(rotation);
                }
            }

            // Switch camera look action active state
            if (DisableCameraLookOnSmoothAngle && CameraLookAction != null)
            {
                var shouldLock = _isSecondaryActionDown;
                var currentActive = CameraLookAction.action.enabled;
                var shouldActive = !shouldLock;
                if (currentActive && !shouldActive && CameraLookAction.action.actionMap.enabled)
                {
                    CameraLookAction.action.Disable();
                }
                else if (!currentActive && shouldActive && CameraLookAction.action.actionMap.enabled)
                {
                    CameraLookAction.action.Enable();
                }
            }
        }

        private void Update()
        {
            _selectedMapAssetChangeEventDetector.Update();
        }

        private void LateUpdate()
        {
            if (Physics.Raycast(Camera.position, Camera.forward, out var hit, RaycastDistance, RaycastMask))
            {
                if (_objectPlacementCalculator != null)
                {
                    _objectPlacementCalculator.SetSnapPoint(hit.point, hit.normal);
                    _objectPlacementCalculator.Calculate(out var position, out var rotation);
                    _mapAssetPreview.SetPosition(position);
                    _mapAssetPreview.SetRotation(rotation);
                }
            }
        }

        private void OnChangeSelectedAsset(MapAssetData asset)
        {
            if (asset == null)
            {
                _objectPlacementCalculator = null;
                _mapAssetPreview.Clear();
            }
            else
            {
                _objectPlacementCalculator = new ObjectPlacementCalculator(asset.Asset);
                _mapAssetPreview.SetMapAsset(asset.GUID, asset.Asset);
            }
        }

        private class SelectedMapAssetChangeEventDetector
        {
            public event System.Action<MapAssetData> OnSelectedAssetChanged;

            private MapAssetData _previousSelectedAsset;

            public void Clear()
            {
                _previousSelectedAsset = null;
            }

            public void Update()
            {
                var selectedMapAsset = MapEditorEnvironment.Instance.Payload.SelectedAsset;
                if (selectedMapAsset != _previousSelectedAsset)
                {
                    _previousSelectedAsset = selectedMapAsset;
                    OnSelectedAssetChanged?.Invoke(selectedMapAsset);
                }
            }
        }

        private class ObjectPlacementCalculator
        {
            private Bounds _targetBounds;

            private float _angle;
            public float Angle => _angle;

            private Vector3 _snapPosition;
            private Vector3 _snapNormal;

            public ObjectPlacementCalculator(GameObject targetObject)
            {
                _targetBounds = ObjectSnapUtility.CollectBounds(targetObject);
            }

            public void SetAngle(float angle)
            {
                _angle = angle;
            }

            public void SetSnapPoint(Vector3 position, Vector3 normal)
            {
                _snapPosition = position;
                _snapNormal = normal;
            }

            public void Calculate(out Vector3 position, out Quaternion rotation)
            {
                position = _snapPosition + _snapNormal * _targetBounds.extents.z; // Dummy
                rotation = Quaternion.AngleAxis(_angle, Vector3.up);
            }
        }

        private class MapAssetPreview
        {
            private string _mapAssetGuid;

            private GameObject _mapObject;

            private readonly List<Collider> _colliderBuffer = new();

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
        }

        private static class ObjectSnapUtility
        {
            public static float GetNearestSnappedAngle(float angle, float snapAngle)
            {
                return Mathf.Round(angle / snapAngle) * snapAngle;
            }

            public static Bounds CollectBounds(GameObject gameObject)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
                foreach (var renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

                return bounds;
            }
        }
    }
}
