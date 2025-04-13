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

            StopAttack();

            _currentTarget = target;

            _attackCoroutine = StartCoroutine(AttackLoop());
        }


        private IEnumerator AttackLoop()
        {
            while (_currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, _currentTarget.GetPosition());
                float adjustedDistance = distance - _currentTarget.GetCollisionRadius();

                if (adjustedDistance <= _unitData.attackRange)
                {
                    _currentTarget.TakeDamage(_unitData.damage);
                }

                yield return new WaitForSeconds(_unitData.attackSpeed);
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