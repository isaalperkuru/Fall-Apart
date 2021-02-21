using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers(Stats stat);
        IEnumerable<float> GetPercentageModifiers(Stats stat);
    }
}

