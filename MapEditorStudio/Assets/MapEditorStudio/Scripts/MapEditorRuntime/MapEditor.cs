using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MapEditorStudio.MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        public PlayerInput PlayerInput;

        public string PlayerActionMapName = "Player";

        public string UIActionMapName = "UI";

        public InputActionReference MenuAction;

        public GameObject UIRoot;

        public MapAssetListUI MapAssetListUI;

        private bool _uiEnabled;

        private void Awake()
        {
            var playerActionMap = PlayerInput.actions.FindActionMap(PlayerActionMapName);
            if (playerActionMap != null)
            {
                playerActionMap.Enable();
            }

            var uiActionMap = PlayerInput.actions.FindActionMap(UIActionMapName);
            if (uiActionMap != null)
            {
                uiActionMap.Enable();
            }
        }

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

        private void Start()
        {
            MapAssetListUI.OnCreateItem += OnCreateMapAssetElementUI;
            MapAssetListUI.OnDeleteItem += OnDeleteMapAssetElementUI;
            var items = MapAssetManager.Instance.GetItemsAll().ToList();
            MapAssetListUI.SetData(items);
            MapEditorEnvironment.Instance.Payload.SelectedAsset = items.FirstOrDefault();
            UpdateUIActive();
            UpdateInputActive();
        }

        private void OnCreateMapAssetElementUI(MapAssetListElementUI item)
        {
            if (item.TryGetComponent<Button>(out var button))
            {
                button.onClick.AddListener(() => OnSelectMapAsset(item.MapAssetData));
            }
        }

        private void OnDeleteMapAssetElementUI(MapAssetListElementUI item)
        {
            if (item.TryGetComponent<Button>(out var button))
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void OnSelectMapAsset(MapAssetData asset)
        {
            MapEditorEnvironment.Instance.Payload.SelectedAsset = asset;
        }

        private void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            if (MenuAction != null && context.action.id == MenuAction.action.id && context.started)
            {
                _uiEnabled = !_uiEnabled;
                UpdateToolActive();
                UpdateUIActive();
                UpdateInputActive();
            }
        }

        private void UpdateToolActive()
        {
            var toolController = MapEditorEnvironment.Instance.ToolController;
            toolController.SetSharedActive(!_uiEnabled);
        }

        private void UpdateUIActive()
        {
            UIRoot.SetActive(_uiEnabled);
        }

        private void UpdateInputActive()
        {
            var playerActionMap = PlayerInput.actions.FindActionMap(PlayerActionMapName);
            if (playerActionMap != null)
            {
                if (_uiEnabled)
                {
                    playerActionMap.Disable();
                }
                else
                {
                    playerActionMap.Enable();
                }
            }
        }
    }
}
