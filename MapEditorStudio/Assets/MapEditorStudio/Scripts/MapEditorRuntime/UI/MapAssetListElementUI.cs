using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapAssetListElementUI : MonoBehaviour
    {
        private MapAssetData _data;

        public void SetData(MapAssetData data)
        {
            _data = data;
        }
    }
}
