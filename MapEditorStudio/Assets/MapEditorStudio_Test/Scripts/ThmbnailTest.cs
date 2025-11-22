using System.Linq;
using MapEditorStudio;
using MapEditorStudio.MapEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditorStudioTest
{
    public class ThmbnailTest : MonoBehaviour
    {
        public Image Image;

        public int Index;

        private void Start()
        {
            var items = MapAssetManager.Instance.GetItemsAll();
            var item = items.ToList()[Index];
            if (item != null)
            {
                var tex = MapAssetThumbnailManager.Instance.GetThumbnail(item);
                Image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }
        }
    }
}
