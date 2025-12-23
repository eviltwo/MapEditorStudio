using System.Collections.Generic;
using UnityEngine;

namespace MapEditorStudio.MapEditor.MapEditTools
{
    public interface IMapEditAction
    {
        string GetName();
        void Execute();
        void Undo();
    }

    public class MapEditActionManager
    {
        private readonly List<IMapEditAction> _actions = new();

        private int _lastExecutedIndex = -1;

        public void ExecuteAction(IMapEditAction action)
        {
            // Remove future actions
            if (_lastExecutedIndex + 1 < _actions.Count)
            {
                _actions.RemoveRange(_lastExecutedIndex + 1, _actions.Count - _lastExecutedIndex - 1);
            }

            _actions.Add(action);
            _lastExecutedIndex = _actions.Count - 1;
            action.Execute();
            Debug.Log($"Execute action: {action.GetName()}");
        }

        public void UndoAction()
        {
            if (_lastExecutedIndex < 0) return;
            _actions[_lastExecutedIndex].Undo();
            Debug.Log($"Undo action: {_actions[_lastExecutedIndex].GetName()}");
            _lastExecutedIndex--;
        }

        public void RedoAction()
        {
            if (_lastExecutedIndex + 1 >= _actions.Count) return;
            _actions[_lastExecutedIndex + 1].Execute();
            Debug.Log($"Redo action: {_actions[_lastExecutedIndex + 1].GetName()}");
            _lastExecutedIndex++;
        }
    }
}
