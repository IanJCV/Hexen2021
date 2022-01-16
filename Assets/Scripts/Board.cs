using DAE.Commons;
using System;

#region EVENTS
public class PlacedEventArgs : EventArgs
{
    public Hex ToPosition { get; }

    public Piece Piece { get; }

    public PlacedEventArgs(Hex toPosition, Piece piece)
    {
        ToPosition = toPosition;
        Piece = piece;
    }
}

public class MovedEventArgs : EventArgs
{
    public Hex ToPosition { get; }
    public Hex FromPosition { get; }
    public Piece Piece { get; }

    public MovedEventArgs(Hex toPosition, Hex fromPosition, Piece piece)
    {
        ToPosition = toPosition;
        FromPosition = fromPosition;
        Piece = piece;
    }
}

public class TakenEventArgs : EventArgs
{
    public Hex FromPosition { get; }
    public Piece Piece { get; }

    public TakenEventArgs(Hex fromPosition, Piece piece)
    {
        FromPosition = fromPosition;
        Piece = piece;
    }
}
#endregion

public class Board
{

    public event EventHandler<PlacedEventArgs> Placed;
    public event EventHandler<MovedEventArgs> Moved;
    public event EventHandler<TakenEventArgs> Taken;

    private BidirectionalDictionary<Hex, Piece> _positionPiece = new BidirectionalDictionary<Hex, Piece>();


    public bool Place(Piece piece, Hex toPosition)
    {
        if (TryGetPieceAt(toPosition, out _))
            return false;

        if (TryGetHexOf(piece, out _))
            return false;

        _positionPiece.Add(toPosition, piece);
        OnPlaced(new PlacedEventArgs(toPosition, piece));

        return true;
    }

    public bool Move(Piece piece, Hex toPosition)
    {
        if (TryGetPieceAt(toPosition, out _))
            return false;

        if (!TryGetHexOf(piece, out var fromPosition) || !_positionPiece.Remove(piece))
            return false;

        _positionPiece.Add(toPosition, piece);
        OnMoved(new MovedEventArgs(toPosition, fromPosition, piece));

        return true;

    }

    public bool Take(Piece piece)
    {
        if (!TryGetHexOf(piece, out var fromPosition))
            return false;

        if (!_positionPiece.Remove(piece))
            return false;

        OnTaken(new TakenEventArgs(fromPosition, piece));
        return true;

    }


    public bool TryGetPieceAt(Hex position, out Piece piece)
        => _positionPiece.TryGetValue(position, out piece);

    public bool TryGetHexOf(Piece piece, out Hex position)
        => _positionPiece.TryGetKey(piece, out position);


    #region EventTriggers
    protected virtual void OnPlaced(PlacedEventArgs eventArgs)
    {
        var handler = Placed;
        handler?.Invoke(this, eventArgs);
    }

    protected virtual void OnMoved(MovedEventArgs eventArgs)
    {
        var handler = Moved;
        handler?.Invoke(this, eventArgs);
    }

    protected virtual void OnTaken(TakenEventArgs eventArgs)
    {
        var handler = Taken;
        handler?.Invoke(this, eventArgs);
    }
    #endregion

}