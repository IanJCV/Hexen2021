using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "DAE/Card Art Set")]
public class CardArtSet : ScriptableObject
{
    public CardType cardType;
    public List<Sprite> sprites = new List<Sprite>();
}