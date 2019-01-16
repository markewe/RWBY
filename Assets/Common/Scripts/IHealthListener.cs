using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthListener {
	void OnTakeDamage();
	void OnHealDamage();
	void OnZeroHealth();
}
