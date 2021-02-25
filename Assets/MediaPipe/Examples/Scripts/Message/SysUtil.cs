using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysUtil
{
    private static long Uid = long.MaxValue;

    public static string GetUid()
    {
        Uid--;
        return Uid.ToString();
    }
}
