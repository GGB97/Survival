using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    private List<IDamageable> thingsToDamage = new List<IDamageable>(); // 리스트말고 HashSet을 사용해도 됨 HashSet은 삽입/삭제 자체가 한번에 일어나기 때문에 속도가 더 빠름

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
        if(other.gameObject.TryGetComponent(out IDamageable damagable)) // damagable 변수에 IDamageable 형식의 컴포넌트가 있다면 가져오고 true 없다면 false
        {
            thingsToDamage.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damagable)) // damagable 변수에 IDamageable 형식의 컴포넌트가 있다면 가져오고 true 없다면 false
        {
            thingsToDamage.Remove(damagable);
        }
    }
}
