using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ItemDragable : MonoBehaviour {
    [SerializeField] private Transform targetItem;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float shiftUpValue;

    private Transform _itemTransform;
    private Collider2D _collider;
    private SpriteRenderer _renderer;
    private int _originLayerID;
    private Vector3 originPos;

    private void Start() {
        _itemTransform = transform;
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _originLayerID = _renderer.sortingLayerID;
        originPos = _itemTransform.position;
    }

    private void OnMouseDown() {
        var touchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        touchPos.z = _itemTransform.position.z;

        _itemTransform.position = touchPos + Vector3.up * shiftUpValue;
        _renderer.sortingLayerID = SortingLayer.NameToID(Constants.TOP_SORTING_LAYER);
    }

    private void OnMouseDrag() {
        var touchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        touchPos.z = _itemTransform.position.z;
        _itemTransform.position = touchPos + Vector3.up * shiftUpValue;
    }

    private void OnMouseUp() {
        var overlappedColliders = new List<Collider2D>();
        var contactFilter = new ContactFilter2D();
        Physics2D.OverlapCollider(_collider, contactFilter, overlappedColliders);

        var isOverTargetArea = overlappedColliders.FirstOrDefault(c => c.CompareTag(Constants.ITEM_TARGET_AREA_TAG));
        if (isOverTargetArea) {
            PlaceInTarget();
        } else {
            _renderer.sortingLayerID = _originLayerID;
            EnableInteraction(false);
            _itemTransform.DOMove(originPos, Constants.ITEM_MOVE_DURATION).SetEase(Ease.OutCubic).OnComplete(delegate { EnableInteraction(true); });
        }
    }

    private void PlaceInTarget() {
        EnableInteraction(false);
        _itemTransform.SetParent(targetItem.parent);
        
        _itemTransform.DOScale(targetItem.localScale, Constants.ITEM_MOVE_DURATION);
        _itemTransform.DOLocalMove(targetItem.localPosition, Constants.ITEM_MOVE_DURATION).SetEase(Ease.OutCubic).OnComplete(delegate {
            targetItem.gameObject.SetActive(true);
            Destroy(gameObject);
        });
    }

    private void EnableInteraction(bool enable) {
        _collider.enabled = enable;
    }
}