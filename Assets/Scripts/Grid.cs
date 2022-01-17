using DAE.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestructionEventArgs : EventArgs
{
    public Hex Hex { get; }

    public DestructionEventArgs(Hex hex)
    {
        Hex = hex;
    }
}

public class Grid
{

    public int Size { get; }
    public float HexRadius { get; }
    public float Offset { get; }

    public event EventHandler<DestructionEventArgs> OnDestruction;
    public event EventHandler<DestructionEventArgs> OnReactivation;

    public Grid(int size, float hexRadius, float offset)
    {
        Size = size;
        HexRadius = hexRadius;
        Offset = offset;
    }

    private BidirectionalDictionary<HexCoords, Hex> _positions = new BidirectionalDictionary<HexCoords, Hex>();

    public bool TryGetPositionAt(HexCoords hex, out Hex position)
        => _positions.TryGetValue(hex, out position);


    public bool TryGetCoordinateOf(Hex position, out HexCoords coordinate)
        => _positions.TryGetKey(position, out coordinate);

    public void ActivateTile(Hex hex)
    {
        hex.Activate();
    }

    public void ActivateTile(HexCoords hex)
    {
        TryGetPositionAt(hex, out var h);
        h.Activate();
    }

    public void ActivateTiles(List<Hex> hexes)
    {
        foreach (var h in hexes)
        {
            h.Activate();
        }
    }

    public void ActivateAllTiles()
    {
        foreach (var h in _positions.Values)
        {
            h.Activate();
        }
    }

    public void DeactivateTile(Hex hex)
    {
        hex.Deactivate();
    }

    public void DeactivateTile(HexCoords hex)
    {
        TryGetPositionAt(hex, out var h);
        h.Deactivate();
    }


    public void DeactivateAllTiles()
    {
        foreach (var h in _positions.Values)
        {
            h.Deactivate();
        }
    }

    // ===================== BOMB STUFF =============================

    public void ReactivateHex(Hex hex)
    {
        Register(hex);
        OnReactivation(this, new DestructionEventArgs(hex));
    }

    public void DestroyHex(Hex hex)
    {
        _positions.Remove(hex);
        OnDestruction(this, new DestructionEventArgs(hex));
    }

    public void DestroyHexes(List<Hex> hexes)
        => hexes.ForEach(h => DestroyHex(h));

    // ===================== BOMB STUFF =============================

    public List<Hex> GetAllPositions()
    {
        return _positions.Values.ToList();
    }

    public Hex GetRandomHex()
    {
        for (int i = 0; i < 20; i++)
        {
            if (TryGetPositionAt(new HexCoords(
                UnityEngine.Random.Range(-Size, Size),
                UnityEngine.Random.Range(-Size, Size)), out var hex))
                return hex;
        }
        throw new Exception("Failed to find a random hex.");
    }

    public void Register(Hex position)
    {
        string[] coordDelim = position.gameObject.name.Split('^');
        var coords = new HexCoords(int.Parse(coordDelim[1]), int.Parse(coordDelim[2]), int.Parse(coordDelim[3]));

        Register(coords, position);
    }

    public void Register(HexCoords coords, Hex position)
    {

#if UNITY_EDITOR
        if (Mathf.Abs(coords.q) > Size)
            throw new ArgumentException(nameof(coords) + coords.ToString());
        if (Mathf.Abs(coords.r) > Size)
            throw new ArgumentException(nameof(coords) + coords.ToString());
        if (Mathf.Abs(coords.s) > Size)
            throw new ArgumentException(nameof(coords) + coords.ToString());
#endif

        _positions.Add(coords, position);
    }
}
