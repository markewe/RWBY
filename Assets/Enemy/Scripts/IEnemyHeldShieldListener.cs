using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHeldShieldListener {
	void OnShieldBroken();
	void OnShieldActiveHit();
	void OnShieldInactiveHit();
	void OnShieldRecharge();
}
