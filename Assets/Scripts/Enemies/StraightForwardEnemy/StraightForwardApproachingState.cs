using UnityEngine;
using UnityEngine.AI;
using UniRx.Triggers;
using UniRx;

namespace Main
{
    public class StraightForwardApproachingState : EnemyBaseState<StraightForwardEnemy>
    {
        private PlayerController _playerController;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Collider _readyToAttackCollider;
        private LayerMask _rayMask;

        private CompositeDisposable _onTriggerStayDis = new CompositeDisposable();

        private float _runSpeed;

        public StraightForwardApproachingState(PlayerController playerController, NavMeshAgent navMeshAgent, Animator animator, float runSpeed, Collider collider, LayerMask rayMask)
        {
            _playerController = playerController;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _runSpeed = runSpeed;
            _readyToAttackCollider = collider;
            _rayMask = rayMask;
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            _onTriggerStayDis?.Clear();

            _readyToAttackCollider.OnTriggerStayAsObservable().Where(t => t.gameObject.GetComponent<PlayerController>()).Subscribe(_ => TryAttack(_, straightForwardEnemy)).AddTo(_onTriggerStayDis);
            _navMeshAgent.speed = _runSpeed;
            _animator.SetBool(Animations.Attack, false);
            _animator.SetBool(Animations.Run, true);
        }


        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            _navMeshAgent.SetDestination(_playerController.transform.position);
        }
        public override void OnTriggerEnter(StraightForwardEnemy enemy, Collider collider)
        {

        }

        private void TryAttack(Collider collider, StraightForwardEnemy straightForwardEnemy)
        {
            Vector3 direction = collider.transform.position - straightForwardEnemy.transform.position;
            Ray ray = new Ray(straightForwardEnemy.transform.position + Vector3.up, direction);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, direction.magnitude + 1, _rayMask))
            {
                if (hitInfo.collider.GetComponent<PlayerController>())
                {
                    Attack(straightForwardEnemy);
                }
            }
        }

        private void Attack(StraightForwardEnemy enemy)
        {
            _onTriggerStayDis?.Clear();
            enemy.ChangeState(enemy.StraightForwardAttackState);
        }
    }
}


