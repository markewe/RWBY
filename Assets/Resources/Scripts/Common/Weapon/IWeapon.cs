using UnityEngine;

public interface IWeapon {
	void OnAttackStart(GameObject target);
	void OnAttackEnd(GameObject target);
}
