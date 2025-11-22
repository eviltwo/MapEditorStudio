using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapAssetListUI : MonoBehaviour
    {
        public MapAssetListElementUI ItemPrefab;

        public RectTransform ItemParent;

        private readonly List<MapAssetData> _data = new();

        private readonly List<MapAssetListElementUI> _items = new();

        private void Reset()
        {
            ItemParent = transform as RectTransform;
        }

        public void SetData(IEnumerable<MapAssetData> data)
        {
            _data.Clear();
            _data.AddRange(data);
            DeleteItems();
            CreateItems();
        }

        private void DeleteItems()
        {
            foreach (var item in _items)
            {
                item.OnEndDisplay();
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void CreateItems()
        {
            var itemNamePrefix = ItemPrefab.name;
            foreach (var data in _data)
            {
                var item = Instantiate(ItemPrefab, ItemParent);
                item.gameObject.name = $"{itemNamePrefix}_{data.Asset.name}";
                item.gameObject.SetActive(true);
                item.SetData(data);
                item.OnBeginDisplay();
                _items.Add(item);
            }
        }
    }
}
