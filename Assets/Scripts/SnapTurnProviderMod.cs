using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapTurnProviderMod : ActionBasedSnapTurnProvider
{
   public Vector2 InputValue => ReadInput();
}
