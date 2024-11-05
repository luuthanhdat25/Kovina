using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISingleton : Singleton<UISingleton>
{
    public EventHandler<IsPauseEvent_Args> IsPauseEvent;
    public EventHandler<IsEndGame_Args> IsEndGame;

    public class IsPauseEvent_Args : EventArgs
    {
        public bool isPause;
    }
    public class IsEndGame_Args: EventArgs
    {
        public int star;
    }
    public void CallOnEndGame(int star)
    {
        IsEndGame?.Invoke(this,new IsEndGame_Args
        {
            star = star
        });
    }
    public void CallOnPauseChanges(bool isPause)
    {
        IsPauseEvent?.Invoke(this,new IsPauseEvent_Args
        {
            isPause = isPause
        });
    }

    
}

