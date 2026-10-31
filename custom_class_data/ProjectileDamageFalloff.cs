using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ProjectilDamageFalloff : MonoBehaviour
{
    private Projectile _projectile;
    private PlayerController _owner;

    private float _normalDamageMaxDistance;
    private float _finalReductionDistance;
    private float _farDamageReduction;

    private float _originalDamage;
    private float _currentMultiplier = 1f;

    private void Start()
    {
        this._projectile = base.GetComponent<Projectile>();
        this._owner = this._projectile.Owner as PlayerController;
    }

    public void Initialize(float normalDamageMaxDistance, float finalReductionDistance, float farDamageReduction)
    {
        _projectile = GetComponent<Projectile>();

        if (_projectile == null)
        {
            Destroy(this);
            return;
        }

        _normalDamageMaxDistance = normalDamageMaxDistance;
        _finalReductionDistance = finalReductionDistance;
        _farDamageReduction = farDamageReduction;

        _originalDamage = _projectile.baseData.damage;
    }

    private void Update()
    {
        if (_projectile == null) { return; }

        float distance = _projectile.GetElapsedDistance();

        if (distance <= _normalDamageMaxDistance)
            return;

        float distanceFactor = Mathf.Clamp01(
            (distance - _normalDamageMaxDistance) /
            (_finalReductionDistance - _normalDamageMaxDistance)
        );

        float multiplier = 1f - (distanceFactor * _farDamageReduction);

        /* Set damage based on its original value. */
        _projectile.baseData.damage = _originalDamage * multiplier;

        /* Apply only the change in scale since the previous update. */
        float scaleChange = multiplier / _currentMultiplier;

        _projectile.RuntimeUpdateScale(scaleChange);

        _currentMultiplier = multiplier;

        /* Stop updating once maximum reduction is reached. */
        if (distance >= _finalReductionDistance)
        {
            enabled = false;
        }
    }
}
