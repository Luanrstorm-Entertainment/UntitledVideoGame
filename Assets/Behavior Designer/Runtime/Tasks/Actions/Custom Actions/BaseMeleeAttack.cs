using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;

public class BaseMeleeAttack : Action
{
    [SerializeField] private SharedEntity unit;
    [SerializeField] private SharedTransform targetSpot;
    [SerializeField] private SharedFloat attackSpeed, attackDelay, damage, currentAttackDelay;

    private Animator animator;
    private MeleeWeapon weapon;

    public override void OnStart()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("AttackSpeed", attackSpeed.Value);
        weapon = gameObject.GetComponentInChildren<MeleeWeapon>();
        weapon.Initialize(unit.Value, damage.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (targetSpot == null || targetSpot.Value.IsDestroyed()) return TaskStatus.Failure;
        try
        {
            Vector3 dir = targetSpot.Value.position - unit.Value.transform.position;
            dir.y = 0f;
            unit.Value.transform.rotation = Quaternion.LookRotation(dir);
        }
        catch (MissingReferenceException e)
        {
            return TaskStatus.Failure;
        }

        currentAttackDelay.Value -= Time.deltaTime;
        if (currentAttackDelay.Value < 0f)
        {
            animator.SetTrigger("Attack");
            currentAttackDelay.SetValue(attackDelay.Value);
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}