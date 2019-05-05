using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AllEnigme 
{
    public static bool IS_MOON_OPEN(MoonPart part1 , MoonPart part2)
    {
        return (part1.Progress == 0 && part2.Progress == 0 && part1.Ratio >= 0.999f && part2.Ratio >= 0.999f);
    }
}
