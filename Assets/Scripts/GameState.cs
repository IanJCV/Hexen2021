using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StateChangeEventArgs : EventArgs
{
    public GameState NewState { get; }

    public StateChangeEventArgs(GameState newState)
    {
        NewState = newState;
    }
}

public abstract class GameState
{
    private bool _active;

    public bool Active
    {
        get { return _active; }
        set 
        { 
            _active = value;
            if (_active == true)
                Activate();
            else
                Deactivate();
        }
    }

    protected GameLoop _loop;
    protected GameSettings _settings;

    public event EventHandler<StateChangeEventArgs> OnStateChange;

    protected GameState(GameLoop loop)
    {
        _loop = loop;
        _settings = _loop.gameSettings;
    }

    protected abstract void Activate();
    protected abstract void Deactivate();
    protected void OnChange(GameState newState)
    {
        var handler = OnStateChange;
        handler?.Invoke(this, new StateChangeEventArgs(newState));
    }
    
}

public class StartScreenState : GameState
{
    public StartScreenState(GameLoop loop) : base(loop)
    {
    }

    protected override void Activate()
    {
        Debug.Log("Start Screen State Activated. Switching to play mode.");
        OnChange(new PlayScreenState(_loop));
    }

    protected override void Deactivate()
    {

    }

    
}

public class PlayScreenState : GameState
{

    private Board _board;
    private Grid _grid;
    private MovementManager _movementManager;
    private PlayerHand _playerHand;

    public PlayerPiece player;

    private Card _currentCard;
    private Hex _currentHex;

    private static PlayScreenState _instance;
    public static PlayScreenState Instance
    {
        get
        {
            if (_instance == null)
                throw new Exception("how???");
            return _instance;
        }
        private set => _instance = value;
    }

    public PlayScreenState(GameLoop loop) : base(loop)
    {
        Instance = this;
    }

    protected override void Activate()
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

        _grid.OnDestruction += OnTileDestroyed;
        _grid.OnReactivation += OnTileReactivated;

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

    protected override void Deactivate()
    {

    }

    #region GAME_LOOP_STUFF


    private void OnTileDestroyed(object sender, DestructionEventArgs e)
    {

        e.Hex.gameObject.SetActive(false);
    }

    private void OnTileReactivated(object sender, DestructionEventArgs e)
    {

        e.Hex.gameObject.SetActive(true);
    }

    private void GeneratePieces()
    {
        int piecesAmount = UnityEngine.Random.Range(4, 8);

        for (int i = 0; i < piecesAmount; i++)
        {
            Hex h = _grid.GetRandomHex();
            if (!_board.TryGetPieceAt(h, out _))
            {
                var p = _loop.InstantiateObject(_settings.piecePrefab).GetComponent<Piece>();
                _board.Place(p, h);
            }
            else
                Debug.Log("Failed placing a piece!");
        }

        player = _loop.InstantiateObject(_settings.playerPrefab).GetComponent<PlayerPiece>();
        _grid.TryGetPositionAt(new HexCoords(0, 0), out var zeroHex);
        _board.Place(player, zeroHex);
        Debug.Log($"Generated {piecesAmount} pieces.");
        Debug.Log($"Generated player piece.");
    }

    public Card CreateNewCard(CardType type)
    {
        Card c = _loop.InstantiateObject(_settings.cardPrefab, _settings.cardHolder.transform).GetComponent<Card>();    
        c.cardType = type;
        c.GetComponent<Image>().sprite = _settings.artSet.sprites[(int)c.cardType];
        return c;
    }

    private void ConnectHexes()
    {
        var hexes = _loop.FindObjectsInScene<Hex>();
        foreach (var h in hexes)
        {

            _grid.Register(h);

            h.OnDropped += OnHexDrop;
            h.OnEnter += OnHexHover;
            h.OnExit += OnHexExit;
        }
    }

    private void BeginCardDrag(Card card)
    {
        _currentCard = card;
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
            
            if (positions.Count > 0)
            {
                foreach (var p in positions)
                {
                    p.Activate();
                }
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
        if (e.Piece == player)
            EndGame();
    }

    private void OnPiecePlaced(object sender, PlacedEventArgs e)
    {
        e.Piece.transform.position = e.ToPosition.transform.position;
    }

    #endregion

    private void EndGame() 
        => OnChange(new EndScreenState(_loop));
}

public class EndScreenState : GameState
{
    public EndScreenState(GameLoop loop) : base(loop)
    {
    }

    protected override void Activate()
    {
        Debug.Log("Game over. Restarting.");
    }

    protected override void Deactivate()
    {

    }

    private void RestartGame()
        => OnChange(new StartScreenState(_loop));
}
