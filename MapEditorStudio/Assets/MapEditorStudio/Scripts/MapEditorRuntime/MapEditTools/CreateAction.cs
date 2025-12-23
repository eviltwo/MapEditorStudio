using UnityEngine;

namespace MapEditorStudio.MapEditor.MapEditTools
{
    public class CreateAction : IMapEditAction
    {
        private readonly GameObject _prefab;
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _scale;
        private GameObject _createdObject;

        public CreateAction(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _prefab = prefab;
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }

        public string GetName() => $"Create {_prefab.name}";

        public void Execute()
        {
            _createdObject = Object.Instantiate(_prefab, _position, _rotation);
            _createdObject.transform.localScale = _scale;
        }

        public void Undo()
        {
            Object.Destroy(_createdObject);
        }
    }
}
