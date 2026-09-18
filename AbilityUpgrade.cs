using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUpgrade : StatUpgrade
{
    public BoolValue upgradeCheck;

    public override void ApplyUpgrade()
    {
        upgradeCheck.RuntimeValue = true;
    }
}
