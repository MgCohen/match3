using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileView: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D box;

    [SerializeField] private float dragMovementAmount = 0.25f;

    public TileData Tile { get; private set; }
    
    private Vector2 startingPoint;

    public void ToggleTouch(bool state)
    {
        box.enabled = state;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startingPoint = Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 pos = (currentPoint - startingPoint).GetUnidirectional(0.5f);
        sprite.transform.localPosition = pos * dragMovementAmount;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 currentPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 pos = (currentPoint - startingPoint).GetUnidirectional01(0.5f);

        var oldPos = transform.position;
        transform.position = sprite.transform.position;
        sprite.transform.localPosition = Vector3.zero;
        transform.DOMove(oldPos, 0.2f);

        if (pos != Vector2.zero)
        {
            BoardEvents.TrySwapTile(Tile, pos);
        }
    }

    public void SetTileData(TileData tile)
    {
        this.Tile = tile;
        sprite.color = tile.color;
    }
}
