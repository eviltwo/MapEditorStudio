using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        public MapAssetListUI MapAssetListUI;

        private void Start()
        {
            var items = MapAssetManager.Instance.GetItemsAll();
            MapAssetListUI.SetData(items);
        }
    }
}
