using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    protected Unit unit;
    protected Transform unitTransform;
    private UnitMovement unitMovement;

    private float currentAttackTime;
    private RaycastHit hit;               
    private Ray ray;
    private int notShootableMask;

    void Awake()
    {
        unit = GetComponent("Unit") as Unit;
        unitTransform = GetComponent<Transform>();
        unitMovement = GetComponent("UnitMovement") as UnitMovement;

        notShootableMask = LayerMask.GetMask("NotShootable");                   // препятствия для стрелков что на этом слое

    }

    public bool Attack(GameObject target, Vector3 vector)                       // вернет true, если враг убит
    {
        unitMovement.Rotate(vector);

        if (Time.time > currentAttackTime)
        {
            currentAttackTime = Time.time + unit.AttackTime;

            // Прицеливание - если прямой наводки нет, идем еще узел до цели, если есть - выстрел (удар)
            ray = new Ray(unitTransform.position + new Vector3(0f, 0.3f, 0f), transform.forward);
            Debug.DrawRay(ray.origin, ray.direction*vector.magnitude, Color.red);     

            if (Physics.Raycast(ray, out hit, vector.magnitude, notShootableMask))
            {
                unit.Targeting();
                Debug.Log(this.name + ": Fucking" + hit.collider);
            }           
            else
            {
                Debug.Log(this.name + ": Attacking!");
                //Debug.DrawRay(ray.origin, ray.direction * vector.magnitude, Color.yellow);
                Bullet(vector);
                // чем дальше, тем меньше урон, но не менее 1
                UnitHP targetHP = target.GetComponent<UnitHP>();
                if (targetHP.TakeDamage(Mathf.Max(Random.Range(unit.DamageBaseMin, unit.DamageBaseMax) / vector.magnitude, 1f))) return true;
            }
        }
        return false;
    }

    protected virtual void Bullet(Vector3 vector) {}

    protected virtual void FixedUpdate() {}
}
