using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
   public static FishManager instance { get; private set; }

    [SerializeField] private List <Transform> randomFishSpawnningPointList = new();
    private int Rand;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void PlaceFishAtRandomPoints(SwimmableContext context)
    {
        context.gameObject.SetActive(false);
        if(context.FishMoveType == FishMovementType.Static)
        {
            Destroy(context.gameObject);
            return;
        }
        Rand = UnityEngine.Random.Range(0, randomFishSpawnningPointList.Count - 1);
        context.transform.position = randomFishSpawnningPointList[Rand].position;
        context.gameObject.SetActive(true);
    }
}
