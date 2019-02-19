using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable {
    event EventHandler InflictEvent;
    void InflictDamage(int inDamageAmount);
}