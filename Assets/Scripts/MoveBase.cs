using DAE.Commons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class MoveBase
{
    protected Grid _grid;
    protected Board _board;

    protected MoveBase(Grid grid, Board board)
    {
        _grid = grid;
        _board = board;
    }

    public abstract List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g);
    public abstract List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g);
    public abstract void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g);

    private List<Hex> _validPositions = new List<Hex>();

    public MoveBase NorthEast(Hex hex, int numTiles = int.MaxValue)
        => Move(1, -1, 0, hex, numTiles);

    public MoveBase East(Hex hex, int numTiles = int.MaxValue)
        => Move(1, 0, -1, hex, numTiles);

    public MoveBase SouthEast(Hex hex, int numTiles = int.MaxValue)
        => Move(0, 1, -1, hex, numTiles);
    public MoveBase SouthWest(Hex hex, int numTiles = int.MaxValue)
        => Move(-1, 1, 0, hex, numTiles);

    public MoveBase West(Hex hex, int numTiles = int.MaxValue)
        => Move(-1, 0, 1, hex, numTiles);

    public MoveBase NorthWest(Hex hex, int numTiles = int.MaxValue)
        => Move(0, -1, 1, hex, numTiles);


    public MoveBase IsolatedNorthEast(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(1, -1, 0, hex, numTiles);

    public MoveBase IsolatedEast(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(1, 0, -1, hex, numTiles);

    public MoveBase IsolatedSouthEast(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(0, 1, -1, hex, numTiles);
    public MoveBase IsolatedSouthWest(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(-1, 1, 0, hex, numTiles);

    public MoveBase IsolatedWest(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(-1, 0, 1, hex, numTiles);

    public MoveBase IsolatedNorthWest(Hex hex, int numTiles = int.MaxValue)
        => IsolatedMove(0, -1, 1, hex, numTiles);


    public MoveBase Any()
    {
        var alltiles = _grid.GetAllPositions();
        _validPositions.AddRange(alltiles);
        return this;
    }

    public MoveBase Single(Hex hex)
    {
        _validPositions.Add(hex);
        return this;
    }

    public List<Hex> Collect()
    {
        return _validPositions;
    }
    public MoveBase Move(int qOffset, int rOffset, int sOffset, Hex hex, int numTiles = int.MaxValue)
    {
        if (!_board.TryGetHexOf(PlayScreenState.Instance.player, out var position))
            return this;

        if (!_grid.TryGetCoordinateOf(position, out var coordinate))
            return this;

        var nextQCoordinate = coordinate.q + qOffset;
        var nextRCoordinate = coordinate.r + rOffset;
        var nextSCoordinate = coordinate.s + sOffset;

        var hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out var nextPosition);


        int step = 0;
        while (hasNextPosition && step < numTiles)
        {
            _validPositions.Add(nextPosition);
            
            nextQCoordinate += qOffset;
            nextRCoordinate += rOffset;
            nextSCoordinate += sOffset;

            hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out nextPosition);

            step++;
        }


        return this;
    }
    public MoveBase MoveOther(int qOffset, int rOffset, int sOffset, Piece piece, int numTiles = int.MaxValue)
    {
        if (!_board.TryGetHexOf(piece, out var position))
            return this;

        if (!_grid.TryGetCoordinateOf(position, out var coordinate))
            return this;

        var nextQCoordinate = coordinate.q + qOffset;
        var nextRCoordinate = coordinate.r + rOffset;
        var nextSCoordinate = coordinate.s + sOffset;

        var hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out var nextPosition);


        int step = 0;
        while (hasNextPosition && step < numTiles)
        {
            _validPositions.Add(nextPosition);

            nextQCoordinate += qOffset;
            nextRCoordinate += rOffset;
            nextSCoordinate += sOffset;

            hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out nextPosition);

            step++;
        }


        return this;
    }

    public MoveBase IsolatedMove(int qOffset, int rOffset, int sOffset, Hex hex, int numTiles = int.MaxValue)
    {
        if (!_board.TryGetHexOf(PlayScreenState.Instance.player, out var position))
            return this;

        if (!_grid.TryGetCoordinateOf(position, out var coordinate))
            return this;

        List<Hex> testPositions = new List<Hex>();

        var nextQCoordinate = coordinate.q + qOffset;
        var nextRCoordinate = coordinate.r + rOffset;
        var nextSCoordinate = coordinate.s + sOffset;

        var hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out var nextPosition);


        int step = 0;
        while (hasNextPosition && step < numTiles)
        {
            testPositions.Add(nextPosition);

            nextQCoordinate += qOffset;
            nextRCoordinate += rOffset;
            nextSCoordinate += sOffset;

            hasNextPosition = _grid.TryGetPositionAt(new HexCoords(nextQCoordinate, nextRCoordinate, nextSCoordinate), out nextPosition);

            step++;
        }

        if (testPositions.Contains(hex))
        {
            _validPositions.AddRange(testPositions);
            return this;
        }
        else
            return this;  
    }

    public MoveBase GetNeighbors(Hex hex)
    {
        _grid.TryGetCoordinateOf(hex, out var position);
        _validPositions.Add(hex);

        for (int i = 0; i < 6; i++)
        {
            var h = HexCoords.Neighbour(position, i);
            _grid.TryGetPositionAt(h, out var nPos);
            _validPositions.Add(nPos);
        }

        return this;
    }

}

public class TeleportMove : MoveBase
{
    public TeleportMove(Grid grid, Board board) : base(grid, board)
    {
        this._grid = grid;
        this._board = board;
    }

    public override void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g)
    {
        var hex = positions[0];
        var player = PlayScreenState.Instance.player;

        b.Move(player, hex);
    }

    public override List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerHex);
        return new TeleportMove(g, b).Single(position)
            .Collect()
            .Where(p => p != playerHex)
            .Where(p => !b.TryGetPieceAt(p, out _))
            .ToList();
    }

    public override List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerHex);
        return new TeleportMove(g, b)
            .Any()
            .Collect()
            .Where(p => p != playerHex || !b.TryGetPieceAt(p, out _))
            .ToList(); ;
    }
}

public class SwipeMove : MoveBase
{
    public SwipeMove(Grid grid, Board board) : base(grid, board)
    {
        this._grid = grid;
        this._board = board;
    }

    public override void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g)
    {
        foreach (var p in positions)
        {
            if (b.TryGetPieceAt(p, out var piece))
            {
                b.Take(piece);
            }
        }
    }

    public override List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerPos);
        g.TryGetCoordinateOf(playerPos, out var playerCoords);
        g.TryGetCoordinateOf(position, out var posCoords);
        g.TryGetPositionAt(HexCoords.RotateHexClockwise(playerCoords, posCoords), out var clockwise);
        g.TryGetPositionAt(HexCoords.RotateHexCounterClockwise(playerCoords, posCoords), out var counterClockwise);
        return new SwipeMove(g, b)
            .NorthEast(position, 1)
            .NorthWest(position, 1)
            .SouthEast(position, 1)
            .SouthWest(position, 1)
            .East(position, 1)
            .West(position, 1)
            .Collect()
            .Where(p => p != playerPos)
            .Where(p => p == position || p == clockwise || p == counterClockwise)
            .ToList();
    }


    public override List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerPos);
        return new SwipeMove(g, b)
            .NorthEast(position, 1)
            .NorthWest(position, 1)
            .SouthEast(position, 1)
            .SouthWest(position, 1)
            .East(position, 1)
            .West(position, 1)
            .Collect()
            .Where(p => p != playerPos)
            .ToList();
    }
}

public class SlashMove : MoveBase
{
    public SlashMove(Grid grid, Board board) : base(grid, board)
    {
        _grid = grid;
        _board = board;
    }

    public override void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g)
    {
        foreach (var p in positions)
        {
            if (b.TryGetPieceAt(p, out var piece))
            {
                b.Take(piece);
            }
        }
    }

    public override List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g)
    {
        return new SwipeMove(g, b)
       .IsolatedNorthEast(position)
       .IsolatedNorthWest(position)
       .IsolatedSouthEast(position)
       .IsolatedSouthWest(position)
       .IsolatedEast(position)
       .IsolatedWest(position)
       .Collect();
    }

    public override List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g)
    {
        return new SwipeMove(g, b)
       .NorthEast(position)
       .NorthWest(position)
       .SouthEast(position)
       .SouthWest(position)
       .East(position)
       .West(position)
       .Collect();
    }
}

public class PushbackMove : MoveBase
{
    public PushbackMove(Grid grid, Board board) : base(grid, board)
    {
        _grid = grid;
        _board = board;
    }

    public override void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var hex);
        g.TryGetCoordinateOf(hex, out var playerCoords);


        BidirectionalDictionary<Piece, HexCoords> enemyPositions = new BidirectionalDictionary<Piece, HexCoords>();

        foreach (var p in positions)
        {
            if (b.TryGetPieceAt(p, out var piece))
            {
                g.TryGetCoordinateOf(p, out var eCoords);
                enemyPositions.Add(piece, eCoords);
            }
        }

        foreach (var e in enemyPositions.Keys)
        {
            enemyPositions.TryGetValue(e, out var eCoords);
            var offset = eCoords - playerCoords;
            b.Move(e, MoveOther(offset.q, offset.r, offset.s, e, 1).Collect()[0]);
        }
    }

    public override List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerPos);
        g.TryGetCoordinateOf(playerPos, out var playerCoords);
        g.TryGetCoordinateOf(position, out var posCoords);
        g.TryGetPositionAt(HexCoords.RotateHexClockwise(playerCoords, posCoords), out var clockwise);
        g.TryGetPositionAt(HexCoords.RotateHexCounterClockwise(playerCoords, posCoords), out var counterClockwise);
        return new SwipeMove(g, b)
            .NorthEast(position, 1)
            .NorthWest(position, 1)
            .SouthEast(position, 1)
            .SouthWest(position, 1)
            .East(position, 1)
            .West(position, 1)
            .Collect()
            .Where(p => p != playerPos)
            .Where(p => p == position || p == clockwise || p == counterClockwise)
            .ToList();
    }

    public override List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g)
    {
        b.TryGetHexOf(PlayScreenState.Instance.player, out var playerPos);
        return new SwipeMove(g, b)
            .NorthEast(position, 1)
            .NorthWest(position, 1)
            .SouthEast(position, 1)
            .SouthWest(position, 1)
            .East(position, 1)
            .West(position, 1)
            .Collect()
            .Where(p => p != playerPos)
            .ToList();
    }
}

public class BombMove : MoveBase
{
    public BombMove(Grid grid, Board board) : base(grid, board)
    {
        _grid = grid;
        _board = board;
    }

    public override void ExecuteMove(Card c, List<Hex> positions, Board b, Grid g)
    {
        foreach (var p in positions)
        {
            if (b.TryGetPieceAt(p, out var piece))
            {
                b.Take(piece);
            }
            g.DestroyHex(p);
        }
    }

    public override List<Hex> ValidIsolatedPositions(Card c, Hex position, Board b, Grid g)
    {
        return new BombMove(g, b)
            .GetNeighbors(position)
            .Collect();
    }

    public override List<Hex> ValidPositions(Card c, Hex position, Board b, Grid g)
    {
        return new BombMove(g, b)
            .GetNeighbors(position)
            .Collect();
    }
}