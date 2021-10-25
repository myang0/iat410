using System;
using UnityEngine;

public class LeghornShot : YolkUpgrade {
  [SerializeField] private float _cooldownMultiplier;
  [SerializeField] private float _healthCostMultiplier;

  protected override void OnTriggerEnter2D(Collider2D col) {
    if (col.CompareTag("Player")) {
      YolkManager yolkManager = GetYolkManager(col);

      if (yolkManager != null) {
        yolkManager.MultiplyByCooldown(_cooldownMultiplier);
        yolkManager.MultiplyByHealthCost(_healthCostMultiplier);
      }

      YolkPickup();
    }
  }
}