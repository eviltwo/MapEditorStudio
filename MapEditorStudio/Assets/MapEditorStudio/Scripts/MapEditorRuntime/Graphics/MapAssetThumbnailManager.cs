using System.Collections.Generic;
using MapEditorStudio.MapEditor.Graphics;
using UnityEngine;

namespace MapEditorStudio.MapEditor
{
    public class MapAssetThumbnailManager : MonoBehaviour
    {
        public static MapAssetThumbnailManager Instance { get; private set; }

        public ThumbnailCapture Capture;

        private readonly Dictionary<string, Texture2D> _thumbnails = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            foreach (var thumbnail in _thumbnails)
            {
                Destroy(thumbnail.Value);
            }

            _thumbnails.Clear();
        }

        public Texture2D GetThumbnail(MapAssetData data)
        {
            if (_thumbnails.TryGetValue(data.GUID, out var v))
            {
                return v;
            }

            var itemInstance = Instantiate(data.Asset);
            var tex = Capture.Capture(itemInstance);
            Destroy(itemInstance);
            _thumbnails.Add(data.GUID, tex);
            return tex;
        }
    }
}
