using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusBehavior : EnemyBehaviour
{
    public bool isInvulnerable;
    protected override void Awake() {
        CactusHealth cactusHealth = gameObject.GetComponent<CactusHealth>();
        cactusHealth.OnCactusStatusDamage += HandleStatusDamage;

        EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
        enemyBehaviour.OnElectrocuted += HandleElectrocuted;

        Health = cactusHealth;
        
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y, ZcoordinateConsts.Interactable);

        Health.OnPreDeath += (sender, args) => {
            StartCoroutine(FadeOutDeath());
            UpdatePathing();
        };
        
        isTurningEnabled = false;
        SetInvulnerability(isInvulnerable);

        base.Awake();
    }

    private void HandleElectrocuted(object sender, EventArgs e) {
        StartCoroutine(Electrocute());
    }
    protected override IEnumerator AttackPlayer() {
        yield break;
    }

    public void SetInvulnerability(bool invulnerable) {
        if (invulnerable) {
            isInvulnerable = true;
            Health.isInvulnerable = true;
        }
        else {
            isInvulnerable = false;
            Health.isInvulnerable = false;
        }
    }
    
    private void UpdatePathing() {
        var graph = AstarPath.active.data.gridGraph;
        AstarPath.active.Scan();
    }
}
