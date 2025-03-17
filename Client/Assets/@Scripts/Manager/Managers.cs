using UnityEngine;

public class Managers : MonoBehaviour
{
    private static bool s_initialized;
    private static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents

	private EventManager _event = new EventManager();
    private GameManager _game = new GameManager();
	private ObjectManager _object = new ObjectManager();
    private PostManager _post = new PostManager();
    private RankManager _rank = new RankManager();
	private TurnManager _turn = new TurnManager();
    private AdsManager _ads = new AdsManager();

	public static EventManager Event { get { return Instance?._event; } }
	public static GameManager Game { get { return Instance?._game; } }
	public static ObjectManager Object { get { return Instance?._object; } }
	public static PostManager Post { get { return Instance?._post; } }
    public static RankManager Rank { get { return Instance?._rank; } }
	public static TurnManager Turn { get { return Instance?._turn; } }
    public static AdsManager Ads { get { return Instance?._ads; } }

	#endregion

	#region Core

	private BackendManager _backend = new BackendManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    public static BackendManager Backend { get { return Instance?._backend; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }

    #endregion

    public static void Init()
    {
        if (s_instance == null && s_initialized == false)
        {
            s_initialized = true;

            GameObject gameObject = GameObject.Find("@Managers");
            if (gameObject == null)
            {
                gameObject = new GameObject { name = "@Managers" };
                gameObject.AddComponent<Managers>();
            }

            DontDestroyOnLoad(gameObject);
            s_instance = gameObject.GetComponent<Managers>();
        }
    }

    public static void Clear()
    {
        s_instance._object.Clear();
    }
}
