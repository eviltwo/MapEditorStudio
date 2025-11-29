using UnityEngine;
using UnityEngine.UI;

namespace MapEditorStudio.MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        public MapAssetListUI MapAssetListUI;

        private void Start()
        {
            MapAssetListUI.OnCreateItem += OnCreateMapAssetElementUI;
            MapAssetListUI.OnDeleteItem += OnDeleteMapAssetElementUI;
            var items = MapAssetManager.Instance.GetItemsAll();
            MapAssetListUI.SetData(items);
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
            Debug.Log($"Select MapAsset : {asset.Asset.name}");
        }
    }
}
