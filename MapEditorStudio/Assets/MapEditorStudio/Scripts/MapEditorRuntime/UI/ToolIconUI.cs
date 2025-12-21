using UnityEngine;
using UnityEngine.UI;

namespace MapEditorStudio.MapEditor.UI
{
    public class ToolIconUI : MonoBehaviour
    {
        [SerializeField]
        private Image BackgroundImage;

        [SerializeField]
        private Image IconImage;

        public Color DefaultColor;

        public Color SelectedColor;

        private bool _isSelected;

        private void Start()
        {
            SetSelected(_isSelected);
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            BackgroundImage.color = _isSelected ? SelectedColor : DefaultColor;
        }

        public void SetIcon(Sprite icon)
        {
            IconImage.sprite = icon;
        }
    }
}
