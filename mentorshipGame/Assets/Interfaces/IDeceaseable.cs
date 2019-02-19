using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IDeceaseable : IDamageable{
    event EventHandler KillEvent;
    void Kill();
}
