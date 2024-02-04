using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    private List<IDamageable> thingsToDamage = new List<IDamageable>(); // ����Ʈ���� HashSet�� ����ص� �� HashSet�� ����/���� ��ü�� �ѹ��� �Ͼ�� ������ �ӵ��� �� ����

    private void Start()
    {
        InvokeRepeating(nameof(DealDamage), 0, damageRate);
    }

    private void DealDamage()
    {
        foreach (var thing in thingsToDamage)
        {
            thing.TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamageable damagable)) // damagable ������ IDamageable ������ ������Ʈ�� �ִٸ� �������� true ���ٸ� false
        {
            thingsToDamage.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damagable)) // damagable ������ IDamageable ������ ������Ʈ�� �ִٸ� �������� true ���ٸ� false
        {
            thingsToDamage.Remove(damagable);
        }
    }
}
