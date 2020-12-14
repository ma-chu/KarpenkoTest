using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantUnitAttack : UnitAttack
{
    [SerializeField]
    private Transform bulletPrefab;
    private Transform bullet;
    private Vector3 bulletDirection;

    protected override void Bullet(Vector3 vector)
    {
        bullet = Instantiate(bulletPrefab, unitTransform.position, unitTransform.rotation);
        bulletDirection = vector;
        Destroy(bullet.gameObject, vector.magnitude / unit.bulletVelocity);     // неизбежная смерть через время, необходимое для полета к цели
    }

    protected override void FixedUpdate()
    {
        if (bullet != null) bullet.position += bulletDirection.normalized * unit.bulletVelocity * Time.deltaTime;
    }
}
