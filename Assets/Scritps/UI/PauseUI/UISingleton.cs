using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISingleton : Singleton<UISingleton>
{
    public Action OnOpenPauseUI;
    public Action OnClosePauseUI;
}
