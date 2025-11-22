using UnityEngine;
using UnityEngine.UI;

namespace MapEditorStudio.MapEditor
{
    public class MapAssetListElementUI : MonoBehaviour
    {
        public Image ThumbnailImage;

        protected MapAssetData Data;

        private Sprite _generatedSprite;

        public virtual void SetData(MapAssetData data)
        {
            Data = data;
        }

        public virtual void OnBeginDisplay()
        {
            ClearSprite();
            var tex = MapAssetThumbnailManager.Instance.GetThumbnail(Data);
            _generatedSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            ThumbnailImage.sprite = _generatedSprite;
        }

        public virtual void OnEndDisplay()
        {
            ThumbnailImage.sprite = null;
            ClearSprite();
        }

        private void ClearSprite()
        {
            if (_generatedSprite != null)
            {
                Destroy(_generatedSprite);
                _generatedSprite = null;
            }
        }
    }
}
