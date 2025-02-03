using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class LoginRegisterScript : MonoBehaviour
{
    // Serialize Fields to link them with the UI Input Fields
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text messageText;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    // Define the User class that will hold user data
    [System.Serializable]
    public class User
    {
        public string username;
        public string email;
        public int score;

        // Constructor to initialize the user data
        public User(string username, string email, int score = 0)
        {
            this.username = username;
            this.email = email;
            this.score = score;  // Default score is 0
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Initialize Firebase
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Database URL
                app.Options.DatabaseUrl = new System.Uri("https://setup-for-login-register-default-rtdb.asia-southeast1.firebasedatabase.app/");

                // Initialize FirebaseAuth and DatabaseReference
                auth = FirebaseAuth.DefaultInstance;
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    public void OnRegistterButtonClicked()
    {
        string email = emailField.text;
        string password = passwordField.text;
        string username = usernameField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            messageText.text = "Please fill in all fields.";
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error registering: " + task.Exception);
                messageText.text = "Error registering user.";
                return;
            }

            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                // Call SaveUserToDatabase to store user data in Firebase
                SaveUserToDatabase(user.UserId, username, email);
                Debug.LogFormat("User registered: {0} ({1})", user.DisplayName, user.UserId);
                messageText.text = "Registration successful.";
            }
            else
            {
                Debug.LogError("FirebaseUser is null after registration!");
            }
        });
    }

    private void SaveUserToDatabase(string userId, string username, string email)
    {
        // Create the user object to store in the database
        User user = new User(username, email); // Default score is set to 0

        // Convert user object to JSON string
        string json = JsonUtility.ToJson(user);

        // Save user data under "scores" node in Firebase
        dbRef.Child("scores").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data saved successfully for userId: " + userId);
            }
            else
            {
                Debug.LogError("Failed to save user data: " + task.Exception);
            }
        });
    }

    public void OnLoginButtonClicked()
    {
        string email = emailField.text;
        string password = passwordField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        });

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
