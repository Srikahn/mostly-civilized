
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A terrain feature that adds resources but is destroyed by improvements
/// </summary>
public class Feature
{
    public string Name;
    public GameObject Prefab;
    public int MovementCost; // Additional movement cost for units moving here
    public float ExpansionModifier; // Modifier to the expansion desire towards this hex
    public List<Biome> AllowedBiomes; // The biomes in which this feature can spawn

    public delegate int HexProbabilityDelegate(Hex h); //TODO: implement that

    public Feature(string name, GameObject prefab, int movementCost = 0, float expansionModifier = 1)
    {
        Name = name;
        Prefab = prefab;
        MovementCost = movementCost;
        ExpansionModifier = expansionModifier;
    }
}