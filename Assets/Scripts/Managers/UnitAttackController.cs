using System.Collections;
using Core.Interfaces;
using Data.Units;
using UnityEngine;

namespace Managers
{
    public class UnitAttackController : MonoBehaviour
    {
        private UnitData _unitData;
        private Coroutine _attackCoroutine;
        private IAttackable _currentTarget;

        public float AttackRange => _unitData.attackRange;

        public void Initialize(UnitData data)
        {
            _unitData = data;
        }

        public void Attack(IAttackable target)
        {
            if (_currentTarget == target && _attackCoroutine != null)
                return; 

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }

            _currentTarget = target;
            _attackCoroutine = StartCoroutine(AttackLoop());
        }

        private IEnumerator AttackLoop()
        {
            float lastAttackTime = Time.time;
            
            while (_currentTarget != null)
            {
                if (_currentTarget == null || _currentTarget.Equals(null))
                {
                    StopAttack();
                    yield break;
                }

                Vector3 targetPos;
                float collisionRadius;
                try
                {
                    targetPos = _currentTarget.GetPosition();
                    collisionRadius = _currentTarget.GetCollisionRadius();
                }
                catch (System.NullReferenceException)
                {
                    StopAttack();
                    yield break;
                }

                float distance = Vector3.Distance(transform.position, targetPos);
                float adjustedDistance = distance - collisionRadius;

                if (adjustedDistance <= _unitData.attackRange)
                {
                    float currentTime = Time.time;
                    if (currentTime - lastAttackTime >= _unitData.attackSpeed)
                    {
                        try
                        {
                            _currentTarget.TakeDamage(_unitData.damage);
                            lastAttackTime = currentTime;
                        }
                        catch (System.NullReferenceException)
                        {
                            StopAttack();
                            yield break;
                        }
                    }
                }

                yield return null;
            }
        }

        public void StopAttack()
        {
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _attackCoroutine = null;
            _currentTarget = null;
        }
    }
}