using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio
{
    [CreateAssetMenu(fileName = nameof(MapAssetListGroup), menuName = nameof(MapEditorStudio) + "/" + nameof(MapAssetListGroup))]
    public class MapAssetListGroup : MapAssetListBase
    {
        public MapAssetListBase[] ItemLists = Array.Empty<MapAssetListBase>();
        
        public override void GetItems(List<MapAssetData> items)
        {
            foreach (var itemList in ItemLists)
            {
                itemList.GetItems(items);
            }
        }
    }
}
