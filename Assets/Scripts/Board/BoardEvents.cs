using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//i could have used signals, in-class events or any other kind of callbacks
// i decided to use just a static event specific class to be simple without any library, while also keeping decoupling
public static class BoardEvents
{
    public static Action<Board> SetBoard = delegate { };

    public static Action<TileData, Vector2> TrySwapTile = delegate { };

    public static Action<TileOperation, List<TileData>> OnTileOperation = delegate { };
}
