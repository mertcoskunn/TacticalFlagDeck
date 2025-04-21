using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    public bool IsLoggedIn  { get; private set;} = false ; 
    public string AuthToken { get; private set;}
    public bool IsInMatch  { get; private set;} = false; 
    public string MatchID { get; private set;}
    public int CurrentTeam { get; private set;} = -1; 

    



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetAuthToken(string token)
    {
        if(!IsLoggedIn)
        {
            IsLoggedIn = true; 
            AuthToken = token; 
        }
        
    }
    public void SetMatchID(string id)
    {   
        if(!IsInMatch)
        {
            IsInMatch = true;  
            MatchID = id;
            Debug.Log("match id setttt");
        }
        else
        {
            Debug.Log("Error: Player is in match"); 
        }
        
    }
    public void ClearMatchID()
    {   
        IsInMatch = false; 
        MatchID = ""; 
    }
    public void SetCurrentTeam(int team)
    {
        CurrentTeam = team; 
    }
    public void ClearCurrentTeam()
    {
        CurrentTeam = -1; 
    }
}
