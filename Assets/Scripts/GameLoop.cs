using System;
using System.Collections;
using System.IO;
using UnityEngine;
using DAE.Commons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

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

    public GameSettings gameSettings;

    [SerializeField]
    private GameObject _startGameScreen;
    [SerializeField]
    private GameObject _endGameScreen;

    [HideInInspector]
    public PlayerPiece player;
    [SerializeField]
    private CardArtSet _artSet;

    private GameState _currentGameState;

    private void Awake()
    {
        _startGameScreen.SetActive(true);

        gameSettings.cardHolder = cardHolder;
        gameSettings.cardPrefab = cardPrefab;
        gameSettings.piecePrefab = piecePrefab;
        gameSettings.playerPrefab = playerPrefab;
        gameSettings.artSet = _artSet;

        _currentGameState = new StartScreenState(this);
        _currentGameState.OnStateChange += OnStateChanged;
        _currentGameState.Active = true;

    }

    private void OnStateChanged(object sender, StateChangeEventArgs e)
    {
        _currentGameState.OnStateChange -= OnStateChanged;
        _currentGameState.Active = false;

        _currentGameState = e.NewState;
        _currentGameState.OnStateChange += OnStateChanged;
        _currentGameState.Active = true;
    }

    public T InstantiateObject<T> (T o) where T : UnityEngine.Object
    {
        return Instantiate(o);
    }

    public T InstantiateObject<T>(T o, Transform parent) where T : UnityEngine.Object
    {
        return Instantiate(o, parent, false);
    }

    public T[] FindObjectsInScene<T>() where T : UnityEngine.Object
    {
        return FindObjectsOfType<T>();
    }

    [HideInInspector]
    public UnityEvent OnGameStarted = new UnityEvent();

    public void OnGameStart()
    {
        _startGameScreen.SetActive(false);
        OnGameStarted.Invoke();
    }

    public void OnGameEnd()
    {
        _endGameScreen.SetActive(true);
    }

}
