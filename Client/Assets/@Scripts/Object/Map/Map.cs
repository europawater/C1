using UnityEngine;
using static Define;

public class Map : BaseObject
{
    [SerializeField]
    private Parallax _foreground;
    [SerializeField]
    private Parallax _midground;
    [SerializeField]
    private Parallax _background;

    private EMapState _mapState = EMapState.None;
    public EMapState MapState
    {
        get { return _mapState; }
        set
        {
            if (_mapState == value)
            {
                return;
            }

            _mapState = value;

            switch (_mapState)
            {
                case EMapState.Stop:
                    _foreground.Speed = 0.0f;
                    _midground.Speed = 0.0f;
                    _background.Speed = 0.0f;
                    break;
                case EMapState.Move:
                    _foreground.Speed = 0.30f;
                    _midground.Speed = 0.03f;
                    _background.Speed = 0.01f;
                    break;

                default:
                    _foreground.Speed = 0.0f;
                    _midground.Speed = 0.0f;
                    _background.Speed = 0.0f;
                    break;
            }
        }
    }

    protected override void Init()
    {
        GameObjectType = EGameObjectType.Map;
        MapState = EMapState.Stop;
    }
}
