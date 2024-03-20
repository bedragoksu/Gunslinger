using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New Charcter", menuName = "Charcter")]
public class Character : ScriptableObject
{
    public Sprite characterImage;
    public string identify;
    public Sprite gunImage;
    public int range;
    public int maxBullet;
}
