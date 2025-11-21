using System;
using UnityEngine;

namespace MapEditorStudio
{
    public class MapAssetManagerComponent : MonoBehaviour
    {
        public MapAssetListBase[] MapAssetLists = Array.Empty<MapAssetListBase>();

        public MapAssetManager MapAssetManager { get; private set; }

        private void Awake()
        {
            MapAssetManager = new MapAssetManager(MapAssetLists);
        }
    }
}
