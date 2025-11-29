using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapAssetListUI : MonoBehaviour
    {
        public MapAssetListElementUI ItemSource;

        public RectTransform ItemParent;

        private readonly List<MapAssetData> _data = new();

        private readonly List<MapAssetListElementUI> _items = new();

        public delegate void CreateItemCallback(MapAssetListElementUI item);

        public event CreateItemCallback OnCreateItem;

        public delegate void DeleteItemCallback(MapAssetListElementUI item);

        public event DeleteItemCallback OnDeleteItem;

        private void Reset()
        {
            ItemParent = transform as RectTransform;
        }

        private void Awake()
        {
            if (ItemSource.gameObject.scene.IsValid())
            {
                ItemSource.gameObject.SetActive(false);
            }
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
                OnDeleteItem?.Invoke(item);
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void CreateItems()
        {
            var itemNamePrefix = ItemSource.name;
            foreach (var data in _data)
            {
                var item = Instantiate(ItemSource, ItemParent);
                item.gameObject.name = $"{itemNamePrefix}_{data.Asset.name}";
                item.gameObject.SetActive(true);
                item.SetData(data);
                item.OnBeginDisplay();
                _items.Add(item);
                OnCreateItem?.Invoke(item);
            }
        }
    }
}
