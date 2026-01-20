using UnityEngine;

//[CreateAssetMenu(fileName = "SOItem", menuName = "SpriteAvatar", order = 1)]
public class SO_SpriteAvatar : ScriptableObject
{
    public Sprite[] sprs;

    public Sprite GetAvatar(int id)
    {
        if (id >= sprs.Length || id < 0)
        {
            id = 0;
        }

        return sprs[id];
    }

    [SerializeField] private Sprite[] _teamLogos;
    public Sprite[] TeamLogos => _teamLogos;

    public Sprite GetTeamLogo(int id)
    {
        if (id >= _teamLogos.Length || id < 0)
        {
            id = 0;
        }

        return _teamLogos[id];
    }
}