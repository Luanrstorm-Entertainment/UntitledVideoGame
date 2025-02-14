﻿using System;
using UnityEngine;

namespace Assets.scripts.Models.WaveModels
{
    [Serializable]
    public class EnemySpawnSetting
    {
       [SerializeField] public GameObject Prefab;
       [SerializeField] public float StrengthRating = 1f;
        [SerializeField] [Range(0,2)]public float SpawnBias = 1f;
    }
}