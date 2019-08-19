using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IWeapon
{
    Collider hitbox;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }
    public void OnAttackStart(GameObject target){
        hitbox.enabled = true;
    }
    public void OnAttackEnd(GameObject target){
        hitbox.enabled = false;
    }
}
