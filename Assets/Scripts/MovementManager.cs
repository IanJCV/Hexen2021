using DAE.Commons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementManager
{
    private Grid _grid;
    private Board _board;
    public Dictionary<CardType, MoveBase> moves = new Dictionary<CardType, MoveBase>();

    public MovementManager(Grid grid, Board board)
    {
        _grid = grid;
        _board = board;
        InitializeMoves();
    }

    public void InitializeMoves()
    {
        moves.Add(CardType.Teleport, new TeleportMove(_grid, _board));
        moves.Add(CardType.Swipe, new SwipeMove(_grid, _board));
        moves.Add(CardType.Slash, new SlashMove(_grid, _board));
        moves.Add(CardType.Pushback, new PushbackMove(_grid, _board));
    }


}