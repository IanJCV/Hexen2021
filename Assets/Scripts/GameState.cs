using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected abstract void Activate();
    protected abstract void Deactivate();
    
}

public class StartScreenState : GameState
{
    protected override void Activate()
    {

    }

    protected override void Deactivate()
    {

    }
}

public class PlayScreenState : GameState
{
    protected override void Activate()
    {

    }

    protected override void Deactivate()
    {

    }
}

public class EndScreenState : GameState
{
    protected override void Activate()
    {

    }

    protected override void Deactivate()
    {

    }
}