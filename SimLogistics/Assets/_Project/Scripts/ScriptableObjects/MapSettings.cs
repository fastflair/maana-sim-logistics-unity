using UnityEngine;

[CreateAssetMenu]
public class MapSettings : ScriptableObject
{
    public string mapName;
    public float tileSizeX;
    public float tileSizeZ;
    public float startY;
    public float spawnDelay;
}
