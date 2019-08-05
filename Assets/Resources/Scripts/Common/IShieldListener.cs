using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShieldListener {
	void OnShieldBroken();
	void OnShieldActiveHit();
	void OnShieldInactiveHit(float hitAmount);
	void OnShieldRecharge();
}
