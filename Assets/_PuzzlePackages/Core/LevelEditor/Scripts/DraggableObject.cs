using System;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class DraggableObject : MonoBehaviour
    {
        private Vector3 _offset;

        private event Action OnMouseDown;
        private event Action OnMouseUp;
        private event Action<Vector3> OnPositionChanged;

        private bool _isDown;

        private Camera _camera;

        private Camera Cam
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.transform.IsChildOf(transform))
                    {
                        _offset = transform.position - GetMouseWorldPos();
                        OnMouseDown?.Invoke();
                        _isDown = true;
                    }
                }
            }
            
            if (!_isDown) return;
            
            if (Input.GetMouseButton(0))
            {
                Vector3 newPosition = GetMouseWorldPos() + _offset;
                if (transform.position != newPosition)
                {
                    transform.position = newPosition;
                    OnPositionChanged?.Invoke(newPosition);
                }
            }
            
            if (!Input.GetMouseButtonUp(0)) return;
            _offset = Vector3.zero;
            OnMouseUp?.Invoke();
            _isDown = false;
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Cam.WorldToScreenPoint(transform.position).z;
            return Cam.ScreenToWorldPoint(mousePos);
        }

        public DraggableObject SetOnMouseDown(Action action)
        {
            OnMouseDown = action;
            return this;
        }

        public DraggableObject SetOnMouseUp(Action action)
        {
            OnMouseUp = action;
            return this;
        }

        public DraggableObject SetOnPositionChanged(Action<Vector3> action)
        {
            OnPositionChanged = action;
            return this;
        }
    }
}