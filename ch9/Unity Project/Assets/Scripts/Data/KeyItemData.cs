using UnityEngine;

[CreateAssetMenu(fileName = "New KeyItemData", menuName = "ScriptableObjects/KeyItemData")]
public class KeyItemData : ScriptableObject
{
    public int Id;
    public string Name;

    // Set a default color so that alpha value is not zero.
    public Color Color = Color.white;
    public Sprite Sprite;


    private void OnValidate() => Id = GetHashCode();
}