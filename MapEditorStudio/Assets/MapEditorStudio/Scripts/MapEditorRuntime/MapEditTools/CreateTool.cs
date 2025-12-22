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
            if (PrimaryAction != null && context.action.id == PrimaryAction.action.id && context.started)
            {
                if (Physics.Raycast(Camera.position, Camera.forward, out var hit, RaycastDistance, RaycastMask))
                {
                    var selectedMapAsset = MapEditorEnvironment.Instance.Payload.SelectedAsset;
                    if (selectedMapAsset == null) return;
                    Instantiate(selectedMapAsset.Asset, hit.point, CalculateRotation());
                }
            }

            if (SecondaryAction != null && context.action.id == SecondaryAction.action.id && context.started)
            {
                _angle += SnapAngle;
                _mapAssetPreview.SetRotation(CalculateRotation());
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
