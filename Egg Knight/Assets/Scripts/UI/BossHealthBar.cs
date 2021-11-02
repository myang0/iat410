using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour {
  [SerializeField] private Slider _slider;

  [SerializeField] private GameObject _barFill;
  [SerializeField] private GameObject _barBorder;
  [SerializeField] private GameObject _barIcon;

  [SerializeField] private GameObject _nameText;

  private void Awake() {
    BroccoliBehaviour.OnBroccoliSpawn += HandleBossSpawn;
    BroccoliHealth.OnBroccoliDamage += HandleHealthChange;
    BroccoliHealth.OnBroccoliDeath += HandleBossDeath;
  }

  private void HandleHealthChange(object sender, HealthChangeEventArgs e) {
    _slider.value = e.newPercent;
  }

  private void HandleBossSpawn(object sender, BossSpawnEventArgs e) {
    _barFill?.SetActive(true);
    _barBorder?.SetActive(true);
    _barIcon?.SetActive(true);
    _nameText?.SetActive(true);

    _nameText.GetComponent<TextMeshProUGUI>().text = e.name;
  }

  private void HandleBossDeath(object sender, EventArgs e) {
    _barFill?.SetActive(false);
    _barBorder?.SetActive(false);
    _barIcon?.SetActive(false);
    _nameText?.SetActive(false);
  }
}