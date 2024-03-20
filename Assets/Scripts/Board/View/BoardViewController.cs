using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardViewController : MonoBehaviour
{
    [SerializeField] private Transform boardHolder;
    [SerializeField] private BoardConfig config;
    [SerializeField] private BoardAnimationController animationController;
    [SerializeField] private Transform boardMask;

    //private Dictionary<Vector2, TileView> board = new Dictionary<Vector2, TileView>();
    private List<TileView> currentTiles = new List<TileView>();
    private Board board;

    private void Awake()
    {
        ToggleView(true);
        animationController.SequenceStarted += () => ToggleTileInteraction(false);
        animationController.SequenceEnded += () => ToggleTileInteraction(true);
    }

    public void ToggleView(bool state)
    {
        if (state)
        {
            BoardEvents.SetBoard += BuildBoardView;
            BoardEvents.OnTileOperation += DoTileOperation;
        }
        else
        {
            BoardEvents.SetBoard -= BuildBoardView;
            BoardEvents.OnTileOperation -= DoTileOperation;
        }
    }

    public void BuildBoardView(Board board)
    {
        this.board = board;

        ClearBoard();
        AdjustCamera();

        boardHolder.transform.position = CalculateOffset();
        SetBoardMask();

        foreach (var tile in board.GetPairs())
        {
            Vector2 index = tile.Value;
            CreateTile(tile.Key, index);
        }
    }

    private void AdjustCamera()
    {
        //quick and dirty solution, i would normally take the screen size into consideration to do a proper fit
        float maxSize = Mathf.Max(config.BoardSize.x, config.BoardSize.y);
        Camera.main.orthographicSize = (maxSize * 1.2f);
    }

    private void SetBoardMask()
    {
        boardMask.transform.position = Vector3.zero;
        boardMask.localScale = config.BoardSize;
    }

    private Vector2 CalculateOffset()
    {
        float xOffset = config.BoardSize.x % 2 == 0 ? (config.BoardSize.x - 1) / 2 : (config.BoardSize.x - 1) / 2;
        float yOffset = config.BoardSize.y % 2 == 0 ? (config.BoardSize.y - 1) / 2 : (config.BoardSize.y - 1) / 2;
        return new Vector2(-xOffset, -yOffset);
    }

    private TileView CreateTile(TileData tile, Vector2 pos, bool show = true)
    {
        //add factory here
        var view = Instantiate(config.TilePrefab, boardHolder);
        view.SetTileData(tile);
        view.transform.localPosition = pos;
        view.gameObject.SetActive(show);
        currentTiles.Add(view);
        return view;
    }

    private void ClearBoard()
    {
        foreach (var tile in currentTiles)
        {
            Destroy(tile.gameObject);
        }
        currentTiles.Clear();
    }

    private void ToggleTileInteraction(bool state)
    {
        foreach (var tile in currentTiles)
        {
            tile.ToggleTouch(state);
        }
    }

    private void DoTileOperation(TileOperation operation, List<TileData> tileList)
    {
        List<TileView> tileViews = currentTiles.Where(t => tileList.Contains(t.Tile)).ToList();
        List<TileView> newTiles = tileList.Except(tileViews.Select(t => t.Tile)).Select(t => CreateTile(t, board.GetIndex(t), false)).ToList();

        IEnumerator sequence = operation switch
        {
            TileOperation.Swap => SwapTiles(tileViews),
            TileOperation.Destroy => DestroyTiles(tileViews),
            TileOperation.Fill => FillTiles(tileViews, newTiles),
            _ => throw new NotImplementedException()
        };

        animationController.QueueSequence(sequence);
    }

    private IEnumerator SwapTiles(List<TileView> tiles)
    {
        if (tiles.Count() != 2)
        {
            throw new Exception("Something went wrong - Swaps are 2 tiles only");
        }

        tiles[0].transform.DOKill();
        tiles[1].transform.DOKill();

        Sequence sequence = DOTween.Sequence();
        float animationSpeed = 0.35f;
        sequence.Append(tiles[0].transform.DOLocalMove(board.GetIndex(tiles[0].Tile), animationSpeed));
        sequence.Join(tiles[1].transform.DOLocalMove(board.GetIndex(tiles[1].Tile), animationSpeed));
        yield return sequence.WaitForCompletion();
    }

    private IEnumerator DestroyTiles(List<TileView> tiles)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var tile in tiles)
        {
            var tween = tile.transform.DOPunchScale(Vector3.one * 1.15f, 0.25f);
            tween.OnComplete(() =>
            {
                currentTiles.Remove(tile);
                Destroy(tile.gameObject);
            });
            sequence.Join(tween);
        }
        sequence.AppendInterval(0.25f);
        yield return sequence.WaitForCompletion();
    }

    private IEnumerator FillTiles(List<TileView> tiles, List<TileView> newTiles)
    {
        var columns = newTiles.GroupBy(t => t.transform.localPosition.x);
        foreach(var column in columns)
        {
            foreach(var tile in column)
            {
                tile.transform.localPosition += Vector3.up * column.Count();
                tile.gameObject.SetActive(true);
                tiles.Add(tile);
            }
        }

        Sequence sequence = DOTween.Sequence();
        foreach (var tile in tiles)
        {
            Vector2 currentPos = tile.transform.localPosition;
            Vector2 newPos = board.GetIndex(tile.Tile);
            var distance = (newPos - currentPos).magnitude;
            var tween = tile.transform.DOLocalMove(newPos, 0.1f * distance).SetEase(Ease.Linear);
            sequence.Join(tween);
        }
        yield return sequence.WaitForCompletion();
    }
}
