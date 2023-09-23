using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfFireAttack : Ability
{
    public override void Use(Vector3 spawnPos)
    {
        transform.rotation = Quaternion.Euler((float)-90.0, (float)0.0, (float)0.0);
        float finalLength = m_Info.Range;
        StartCoroutine(ExpandRing(m_Info.Range));
        cc_PS.Play();
    }

    private IEnumerator ExpandRing(float finalLength)
    {
        var shape = GetComponent<ParticleSystem>().shape;
        var coll = GetComponent<SphereCollider>();
        while (shape.radius < finalLength)
        {
            coll.radius += (float) 0.02;
            shape.radius += (float) 0.02;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Enemy"))
        {
            if (other.transform.position.y < 0.2)
            {
                other.GetComponent<EnemyController>().DecreaseHealth(m_Info.Power);
            }
        }
    }
}
