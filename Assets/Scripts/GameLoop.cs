using System;
using System.Collections;
using System.IO;
using UnityEngine;
using DAE.Commons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameLoop : SingletonMonoBehaviour<GameLoop>
{
    [SerializeField]
    private GameObject cardHolder;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private GameObject piecePrefab;
    [SerializeField]
    private GameObject playerPrefab;

    private Board _board;
    private Grid _grid;
    private MovementManager _movementManager;
    private PlayerHand _playerHand;

    [HideInInspector]
    public PlayerPiece player;
    [SerializeField]
    private CardArtSet _artSet;

    private Card _currentCard;
    private Hex _currentHex;

    private void Awake()
    {
        string boardInfojson = File.ReadAllText(Application.dataPath + "/boardinfo.json");
        BoardInfo boardInfo = JsonUtility.FromJson<BoardInfo>(boardInfojson);

        _board = new Board();
        _grid = new Grid(5, boardInfo.hexRadius, boardInfo.offset);
        _movementManager = new MovementManager(_grid, _board);

        //place places a new piece on the board, while move moves an existing one.
        _board.Placed += OnPiecePlaced;
        _board.Moved += OnPieceMoved;
        _board.Taken += OnPieceTaken;

        _playerHand = new PlayerHand();
        _playerHand.cardsInHand.ForEach(card =>
        {
            card.dragBeginEvent.AddListener(BeginCardDrag);
            card.dragEndEvent.AddListener(EndCardDrag);
            Debug.Log($"Registered card of type {card.cardType}.");
        });

        ConnectHexes();
        GeneratePieces();
    }

    private void GeneratePieces()
    {
        int piecesAmount = UnityEngine.Random.Range(4, 8);

        for (int i = 0; i < piecesAmount; i++)
        {
            Hex h = _grid.GetRandomHex();
            if (!_board.TryGetPieceAt(h, out _))
            {
                var p = Instantiate(piecePrefab).GetComponent<Piece>();
                _board.Place(p, h);
            }
            else
                Debug.Log("Failed placing a piece!");
        }

        player = Instantiate(playerPrefab).GetComponent<PlayerPiece>();
        _grid.TryGetPositionAt(new HexCoords(0, 0), out var zeroHex);
        _board.Place(player, zeroHex);
        Debug.Log($"Generated {piecesAmount} pieces.");
        Debug.Log($"Generated player piece.");
    }

    public Card CreateNewCard(CardType type)
    {
        var c = Instantiate(cardPrefab, cardHolder.transform, false).GetComponent<Card>();
        c.cardType = type;
        c.GetComponent<Image>().sprite = _artSet.sprites[(int)c.cardType];
        return c;
    }

    private void ConnectHexes()
    {
        var hexes = FindObjectsOfType<Hex>();
        foreach (var h in hexes)
        {
            string[] coordDelim = h.gameObject.name.Split('^');
            var coords = new HexCoords(int.Parse(coordDelim[1]), int.Parse(coordDelim[2]), int.Parse(coordDelim[3]));

            _grid.Register(coords, h);

            h.OnDropped += OnHexDrop;
            h.OnEnter += OnHexHover;
            h.OnExit += OnHexExit;
        }
    }

    private void BeginCardDrag(Card card)
    {
        _currentCard = card;
        //cardHolder.GetComponentInParent<CanvasGroup>().blocksRaycasts = false;
        if (_movementManager.moves.TryGetValue(_currentCard.cardType, out var move))
        {
            var positions = move.ValidPositions(_currentCard, _currentHex, _board, _grid);
            foreach (var p in positions)
            {
                p.Activate();
            }
        }
    }
    private void EndCardDrag(Card card)
    {

        _currentCard = null;
        _grid.DeactivateAllTiles();
    }

    private void OnHexDrop(object sender, HexEventArgs e)
    {
        if (_currentCard == null)
            return;

        if (_movementManager.moves.TryGetValue(_currentCard.cardType, out var move))
        {
            var positions = move.ValidIsolatedPositions(_currentCard, e.Hex, _board, _grid);
            
            if (!positions.Contains(e.Hex))
                return;
            
            move.ExecuteMove(_currentCard, positions, _board, _grid);
        }

        Debug.Log("dropped card");
        _playerHand.RemoveCard(_currentCard);
        _currentCard.dragBeginEvent.RemoveAllListeners();
        _currentCard.dragEndEvent.RemoveAllListeners();
        _grid.DeactivateAllTiles();
        _currentCard = null;
        var card = _playerHand.DrawCard();
        
        card?.dragBeginEvent.AddListener(BeginCardDrag);
        card?.dragEndEvent.AddListener(EndCardDrag);
    }
    private void OnHexHover(object sender, HexEventArgs e)
    {
        if (_currentCard == null)
            return;
        _currentHex = e.Hex;

        _grid.DeactivateAllTiles();

        if (_movementManager.moves.TryGetValue(_currentCard.cardType, out var move))
        {
            var positions = move.ValidIsolatedPositions(_currentCard, e.Hex, _board, _grid);
            foreach (var p in positions)
            {
                p.Activate();
            }
        }

    }

    private void OnHexExit(object sender, HexEventArgs e)
    {
        _grid.DeactivateAllTiles();
        _currentHex = null;

        if (_currentCard != null)
        {
            if (_movementManager.moves.TryGetValue(_currentCard.cardType, out var move))
            {
                var positions = move.ValidPositions(_currentCard, _currentHex, _board, _grid);
                foreach (var p in positions)
                {
                    p.Activate();
                }
            }
        }
    }

    private void OnPieceMoved(object sender, MovedEventArgs e)
    {
        e.Piece.transform.position = e.ToPosition.transform.position;
    }

    private void OnPieceTaken(object sender, TakenEventArgs e)
    {
        e.Piece.gameObject.SetActive(false);
    }

    private void OnPiecePlaced(object sender, PlacedEventArgs e)
    {
        e.Piece.transform.position = e.ToPosition.transform.position;
    }

    
}
