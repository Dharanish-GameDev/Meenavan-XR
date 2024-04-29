using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniForDomainExpansion : MonoBehaviour
{
    public void DisableDomainExpansionHand()
    {
        DomainExpansion.Instance.OriLeftHand.SetActive(true);
        DomainExpansion.Instance.DomainExpansionLeftHand.SetActive(false);
    }
}
