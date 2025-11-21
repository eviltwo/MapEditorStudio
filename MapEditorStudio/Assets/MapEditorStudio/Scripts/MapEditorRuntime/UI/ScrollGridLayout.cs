using UnityEngine;
using UnityEngine.UI;

namespace MapEditorStudio.MapEditor
{
    public class ScrollGridLayout : LayoutGroup
    {
        public GameObject ItemPrefab;

        [Min(1)]
        public int ItemCount = 10;

        [Min(1)]
        public int ColumnCount = 3;

        public Vector2 CellSize = new(100, 100);

        public Vector2 Spacing = new(10, 10);

        private float _width;

        private float _height;

        private int _rawCount;

        private LayoutElement sampleG;

        public override void CalculateLayoutInputVertical()
        {
            _width = (CellSize.x + Spacing.x) * ColumnCount - Spacing.x;
            SetLayoutInputForAxis(
                _width,
                _width,
                -1, 0);

            _rawCount = Mathf.CeilToInt(ItemCount / (float)ColumnCount);
            _height = (CellSize.y + Spacing.y) * _rawCount - Spacing.y;
            SetLayoutInputForAxis(
                _height,
                _height,
                -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _height);
        }

        public override void SetLayoutVertical()
        {
        }

        public Vector2 GetItemPosition(int index)
        {
            return GetItemPosition(new Vector2Int(index % ColumnCount, index / ColumnCount));
        }

        public Vector2 GetItemCenterPosition(int index)
        {
            return GetItemPosition(index) + new Vector2(CellSize.x / 2, -CellSize.y / 2);
        }

        private Vector2 GetItemPosition(Vector2Int columnRow)
        {
            return new Vector2(
                columnRow.x * (CellSize.x + Spacing.x),
                rectTransform.rect.height - columnRow.y * (CellSize.y + Spacing.y));
        }

        private Vector3 TranslateItemPosition(Vector2 itemPosition)
        {
            return rectTransform.TransformPoint(itemPosition - rectTransform.pivot * rectTransform.rect.size);
        }

        private void OnDrawGizmos()
        {
            for (var i = 0; i < ItemCount; i++)
            {
                var origin = GetItemPosition(i);
                var p0 = TranslateItemPosition(origin);
                var p1 = TranslateItemPosition(origin + new Vector2(CellSize.x, 0));
                var p2 = TranslateItemPosition(origin + new Vector2(CellSize.x, -CellSize.y));
                var p3 = TranslateItemPosition(origin + new Vector2(0, -CellSize.y));
                Gizmos.DrawLine(p0, p1);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p0);
            }
        }
    }
}
