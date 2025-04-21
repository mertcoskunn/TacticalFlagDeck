using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    [SerializeField]
    private LoginClient clientLogin;

    [SerializeField]
    private SignUpClient clientSignUp;

    [SerializeField]
    private MatchMakingClient clientMatchMaking;


    public LoginClient loginClient => clientLogin;
    public SignUpClient signUpClient => clientSignUp;
    public MatchMakingClient matchMakingClient => clientMatchMaking;  

   

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject); 
    }
}
