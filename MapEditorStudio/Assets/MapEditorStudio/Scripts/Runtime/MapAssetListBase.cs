using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio
{
    public abstract class MapAssetListBase : ScriptableObject
    {
        public abstract void GetItems(List<MapAssetData> items);
    }
}
