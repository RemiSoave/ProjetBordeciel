using UnityEngine;

[CreateAssetMenu(menuName = "Projet Bordeciel/Constellation")]
public class ConstellationData : ScriptableObject
{
    public string code;
    public string displayName;
    public Sprite image;
    [TextArea(3, 10)] public string description;
}
