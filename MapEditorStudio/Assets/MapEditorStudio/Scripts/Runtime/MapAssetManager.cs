using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapEditorStudio
{
    public class MapAssetManager : MonoBehaviour
    {
        public static MapAssetManager Instance { get; private set; }

        public MapAssetListBase[] MapAssetLists = System.Array.Empty<MapAssetListBase>();

        private readonly Dictionary<string, MapAssetData> _items = new();

        private void Awake()
        {
            Instance = this;

            var items = new List<MapAssetData>();
            foreach (var itemList in MapAssetLists)
            {
                itemList.GetItems(items);
            }

            foreach (var item in items)
            {
                _items.Add(item.GUID, item);
            }
        }

        public IEnumerable<MapAssetData> GetItemsAll()
        {
            return _items.Values;
        }

        public IEnumerable<MapAssetData> GetItemsWithTags(string[] tags)
        {
            return _items.Values.Where(v => tags.All(t => v.Tags.Contains(t)));
        }
    }
}
