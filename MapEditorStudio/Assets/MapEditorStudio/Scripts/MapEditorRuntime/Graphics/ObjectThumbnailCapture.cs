using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor.Graphics
{
    public class ThumbnailCapture : MonoBehaviour
    {
        public Camera Camera;

        public Vector2Int Resolution = new(128, 128);

        public Vector3 CapturePosition = new(1000, 1000, 1000);

        public Quaternion CameraRotation = Quaternion.Euler(45, 45, 0);

        private readonly List<Renderer> _rendererCache = new();

        public Texture2D Capture(GameObject targetObject)
        {
            using (new ObjectAdjustScope(targetObject.transform, CapturePosition))
            {
                // Calculate object size
                targetObject.GetComponentsInChildren(_rendererCache);
                var bounds = new Bounds(targetObject.transform.position, Vector3.zero);
                foreach (var r in _rendererCache)
                {
                    bounds.Encapsulate(r.bounds);
                }

                // Move camera
                var distance = bounds.extents.magnitude * 2;
                Camera.transform.position = bounds.center + CameraRotation * Vector3.back * distance;
                Camera.transform.rotation = CameraRotation;

                // Render
                var tempRT = RenderTexture.GetTemporary(Resolution.x, Resolution.y, 32);
                Camera.targetTexture = tempRT;
                Camera.Render();

                // Print to Texture
                var beforeActiveRT = RenderTexture.active;
                RenderTexture.active = tempRT;
                var tex = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, Resolution.x, Resolution.y), 0, 0);
                tex.Apply(false, true);
                Camera.targetTexture = null;
                RenderTexture.active = beforeActiveRT;
                RenderTexture.ReleaseTemporary(tempRT);

                return tex;
            }
        }

        private class ObjectAdjustScope : System.IDisposable
        {
            private readonly Transform _transform;
            private readonly Transform _parent;
            private readonly Vector3 _position;
            private readonly Quaternion _rotation;
            private readonly Vector3 _localScale;

            public ObjectAdjustScope(Transform transform, Vector3 adjustPosition)
            {
                _transform = transform;
                _position = _transform.position;
                _rotation = _transform.rotation;
                _localScale = _transform.localScale;
                _parent = _transform.parent;
                _transform.SetParent(null);
                _transform.SetPositionAndRotation(adjustPosition, Quaternion.identity);
                _transform.localScale = Vector3.one;
            }

            public void Dispose()
            {
                if (_transform)
                {
                    _transform.SetParent(_parent);
                    _transform.SetPositionAndRotation(_position, _rotation);
                    _transform.localScale = _localScale;
                }
            }
        }
    }
}
