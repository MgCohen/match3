using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private BoardConfig config;
    private Board board;

    private void Awake()
    {
        BoardEvents.TrySwapTile += SwapTiles;
    }

    private void Start()
    {
        BuildBoard();
    }

    public Board GetCurrentBoard()
    {
        return board;
    }

    public void BuildBoard()
    {
        board = new Board(config.BoardSize);
        for (int i = 0; i < config.BoardSize.x; i++)
        {
            for (int j = 0; j < config.BoardSize.y; j++)
            {
                Vector2 index = new Vector2(i, j);
                CreateTile(index);
            }
        }
        BoardEvents.SetBoard(board);
    }

    public void SwapTiles(TileData tile, Vector2 direction)
    {
        Vector2 sourcePos = board.GetIndex(tile);
        Vector2 targetPos = sourcePos + direction;
        if (!board.TryGetTile(targetPos, out TileData targetTile))
        {
            return;
        }

        List<TileData> targetTiles = new List<TileData>() { tile, targetTile };

        new SwapTileOperation().Execute(board, targetTiles);
        List<Vector2> emptySpaces = new MatchTilesOperation().Execute(board, targetTiles);
        if (emptySpaces.Count > 0)
        {
            new FillTilesOperation().Execute(this, board, emptySpaces);
        }
    }

    public TileData CreateTile(Vector2 index)
    {
        Color tileColor = GetColor();
        TileData tile = new TileData(tileColor);
        board.Set(tile, index);
        return tile;
    }

    private Color GetColor()
    {
        var randomIndex = Random.Range(0, config.PossibleColors.Count);
        return config.PossibleColors[randomIndex];
    }


    public void RestartBoard()
    {
        board.Clear();
        BuildBoard();
    }
}
