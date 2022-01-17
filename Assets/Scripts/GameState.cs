using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Activate();
        }
    }

    protected GameLoop _loop;
    protected GameSettings _settings;

    public EventHandler<StateChangeEventArgs> OnStateChange;

    protected GameState(GameLoop loop)
    {
        _loop = loop;
        _settings = _loop.gameSettings;
    }

    protected abstract void Activate();
    protected abstract void Deactivate();
    
}

public class StartScreenState : GameState
{
    public StartScreenState(GameLoop loop) : base(loop)
    {
    }

    protected override void Activate()
    {
        Debug.Log("Start Screen State Activated. Switching to play mode.");
        OnStateChange(this, new StateChangeEventArgs(new PlayScreenState(_loop)));
    }

    protected override void Deactivate()
    {

    }
}

public class PlayScreenState : GameState
{
    public PlayScreenState(GameLoop loop) : base(loop)
    {
    }

    protected override void Activate()
    {

    }

    protected override void Deactivate()
    {

    }

    private void EndGame() 
        => OnStateChange(this, new StateChangeEventArgs(new EndScreenState(_loop)));
}

public class EndScreenState : GameState
{
    public EndScreenState(GameLoop loop) : base(loop)
    {
    }

    protected override void Activate()
    {

    }

    protected override void Deactivate()
    {

    }
}

public class GameSettings : ScriptableObject
{
    [SerializeField]
    public GameObject cardHolder;
    [SerializeField]
    public GameObject cardPrefab;
    [SerializeField]
    public GameObject piecePrefab;
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public int gridSize = 5;
}