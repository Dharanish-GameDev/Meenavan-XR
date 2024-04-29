using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FishBaseState
{
    public virtual void Enter(SwimmableContext context) { }
    public virtual void Update(SwimmableContext context) { }
    public virtual void Exit(SwimmableContext context) { }
}
