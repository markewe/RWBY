﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWeaponController : MonoBehaviour {
	public abstract void Attack(GameObject target);
}