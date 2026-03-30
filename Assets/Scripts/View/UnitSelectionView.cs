using System;
using Tools;
using UnityEngine;

namespace View
{
    public class UnitSelectionView : MonoBehaviour
    {
        [SerializeField]
        private UnitSelectionManager _selectionManager;
        [SerializeField]
        private RectTransform _selectionRect;
        [SerializeField]
        private Canvas _canvas;
        
        
        private bool _selectionActive = false;
        
        private void Awake()
        {
            _selectionRect.gameObject.SetActive(false);
            _selectionManager.OnStartSelection += StartSelectionHandler;
            _selectionManager.OnEndSelection += EndSelectionHandler;
        }

        private void Update()
        {
            if (!_selectionActive)
            {
                return;
            }

            UpdateSelectionSize();
        }

        private void UpdateSelectionSize()
        {
            var scale = _canvas.transform.localScale.x;
            var rect = _selectionManager.GetSelectionRect();
            _selectionRect.anchoredPosition = rect.position / scale;
            _selectionRect.sizeDelta = rect.size / scale;
        }

        private void StartSelectionHandler()
        {
            _selectionActive = true;
            UpdateVisibility();
            UpdateSelectionSize();
        }
        
        private void EndSelectionHandler()
        {
            _selectionActive = false;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            _selectionRect.gameObject.SetActive(_selectionActive);
        }
    }
}