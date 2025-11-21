using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio
{
    [CreateAssetMenu(fileName = nameof(FolderMapAssetList), menuName = nameof(MapEditorStudio) + "/" + nameof(FolderMapAssetList))]
    public class FolderMapAssetList : MapAssetListBase
    {
        [Header("Collection settings")]
        public string FolderGUID;
        public string[] Tags;
        
        [Header("Results")]
        public MapAssetData[] Items;

        public override void GetItems(List<MapAssetData> items)
        {
            items.AddRange(Items);
        }
    }
}
