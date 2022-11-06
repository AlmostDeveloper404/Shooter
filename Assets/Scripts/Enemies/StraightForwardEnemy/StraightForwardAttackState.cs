using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;

namespace Main
{
    public class StraightForwardAttackState : EnemyBaseState<StraightForwardEnemy>
    {
        private PlayerController _playerController;
        private Animator _animator;
        private Collider _attackRadius;
        private NavMeshAgent _navMeshAgent;
        private float _attackRate;
        private Weapon _enemyWeapon;
        private Vector3 _lookDirection;
        private LayerMask _layerMask;

        private float _timer;

        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();

        public StraightForwardAttackState(PlayerController playerController, Animator animator, Collider attackRadiusCollider, NavMeshAgent navMeshAgent, float attackRate, Weapon weapon, LayerMask layerMask)
        {
            _layerMask = layerMask;
            _playerController = playerController;
            _animator = animator;
            _attackRadius = attackRadiusCollider;
            _navMeshAgent = navMeshAgent;
            _attackRate = attackRate;
            _enemyWeapon = weapon;
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            _onTriggerExitDis?.Clear();

            _timer = 0;
            _attackRadius.OnTriggerExitAsObservable().Where(t => t.gameObject.GetComponent<PlayerController>()).Subscribe(_ => AttackRadiusExit(straightForwardEnemy)).AddTo(_onTriggerExitDis);
            _navMeshAgent.speed = 0;
            _animator.SetBool(Animations.Attack, true);
            _animator.SetBool(Animations.Run, false);
        }
        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            if (!HasDirectView<PlayerController>.HasView(straightForwardEnemy.transform.position, _playerController.transform.position, _layerMask))
            {
                _onTriggerExitDis?.Clear();
                straightForwardEnemy.ChangeState(straightForwardEnemy.ApproachingToAttackState);
            }

            _lookDirection = _playerController.transform.position - straightForwardEnemy.transform.position;
            straightForwardEnemy.transform.rotation = Quaternion.LookRotation(_lookDirection.normalized);

            _timer += Time.deltaTime;
            if (_timer > _attackRate)
            {
                _timer = 0;
                _enemyWeapon.Attack(_playerController.transform);
            }
        }

        public override void OnTriggerEnter(StraightForwardEnemy enemy, Collider collider)
        {

        }

        private void AttackRadiusExit(StraightForwardEnemy straightForwardEnemy)
        {
            _onTriggerExitDis?.Clear();
            straightForwardEnemy.ChangeState(straightForwardEnemy.ApproachingToAttackState);
        }

    }
}


