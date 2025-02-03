using Firebase;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthSingleton : MonoBehaviour
{
    public static FirebaseAuthSingleton Instance { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }

    private FirebaseAuth auth;

    private void Awake()
    {
        // Ensure only one instance of FirebaseAuthSingleton exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this object between scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        // Listen for authentication state changes
        auth.StateChanged += AuthStateChanged;
    }

    // Track authentication state changes
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != CurrentUser)
        {
            bool signedIn = CurrentUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && CurrentUser != null)
            {
                Debug.Log("Signed out");
            }
            CurrentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + CurrentUser.UserId);
            }
        }
    }

    // Make sure to unsubscribe when the object is destroyed
    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }
}
