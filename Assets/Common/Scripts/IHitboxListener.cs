using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxListener {
	void OnWeaponHitboxEnter(WeaponHitbox hitbox);
}
