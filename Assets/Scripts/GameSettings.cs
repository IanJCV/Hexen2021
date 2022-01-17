using UnityEngine;

[CreateAssetMenu(menuName = "DAE/Game Settings")]
public class GameSettings : ScriptableObject
{
    [SerializeField]
    public GameObject cardHolder;
    [SerializeField]
    public GameObject cardPrefab;
    [SerializeField]
    public GameObject piecePrefab;
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public int gridSize = 5;
    [SerializeField]
    public CardArtSet artSet;
}