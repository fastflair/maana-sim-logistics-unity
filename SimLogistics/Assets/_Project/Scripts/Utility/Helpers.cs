using UnityEngine;

public static class Helpers
{
    public static Quaternion QuaternionFromEuler(float x, float y, float z)
    {
        var quaternion = Quaternion.identity;
        quaternion.eulerAngles = new Vector3(x, y, z);
        return quaternion;
    }
}