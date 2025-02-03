using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System.Collections.Generic;

public class LeaderboardScript : MonoBehaviour
{
    // UI elements
    private Transform playerInfoContainer;
    private Transform playerInfoTemplate;
    
    // Firebase Database reference
    private DatabaseReference dbRef;

    // A list to hold the leaderboard data (this will store user entries with score)
    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

    private void Awake()
    {
        // Get references to the UI container and template
        playerInfoContainer = transform.Find("EntryContainer");
        playerInfoTemplate = transform.Find("EntryContainer/EntryTemplate");

        // Set the template inactive (it will be cloned and activated)
        playerInfoTemplate.gameObject.SetActive(false);


        // Firebase is initialized, now access the database
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                // Fetch the top 10 scores from Firebase
                RetrieveTopScores();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    // Retrieve top 10 scores from the database, ordered by score
    private void RetrieveTopScores()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("scores") // Reference to the "scores" node
            .OrderByChild("score") // Order by "score" field
            .LimitToLast(10) // Limit to 10 entries (top 10 scores)
            .ValueChanged += HandleValueChanged; // Subscribe to the ValueChanged event
    }

    // Callback method to handle the Firebase data retrieval
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error: " + args.DatabaseError.Message);
            return;
        }

        // Process the snapshot data (convert to LeaderboardEntry objects)
        if (args.Snapshot.Exists)
        {
            leaderboard.Clear(); // Clear the existing data before adding new data

            foreach (var childSnapshot in args.Snapshot.Children)
            {
                string userId = childSnapshot.Key;
                string username = childSnapshot.Child("username").Value.ToString();
                string email = childSnapshot.Child("email").Value.ToString();
                int score = int.Parse(childSnapshot.Child("score").Value.ToString());

                // Create a LeaderboardEntry object and add it to the list
                leaderboard.Add(new LeaderboardEntry(userId, username, email, score));
            }

            // Sort the leaderboard by score in descending order
            leaderboard.Sort((entry1, entry2) => entry2.score.CompareTo(entry1.score));

            // Display the top 10 leaderboard entries
            DisplayLeaderboard();
        }
        else
        {
            Debug.LogError("No data found.");
        }
    }

    // Method to display the top 10 leaderboard entries in the UI
    private void DisplayLeaderboard()
    {
        float templateHeight = 80f;

        // Iterate through the leaderboard list and instantiate the UI elements
        for (int i = 0; i < leaderboard.Count; i++)
        {
            LeaderboardEntry entry = leaderboard[i];

            // Instantiate a new UI element for each leaderboard entry
            Transform playerInfoTransform = Instantiate(playerInfoTemplate, playerInfoContainer);
            RectTransform playerInfoRectTransform = playerInfoTransform.GetComponent<RectTransform>();
            playerInfoRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i); // Adjust positioning
            playerInfoTransform.gameObject.SetActive(true);

            // Get the TextMeshPro components for rank, username, and score
            TMP_Text[] texts = playerInfoTransform.GetComponentsInChildren<TMP_Text>();

            // Set the values for rank, username, and score
            texts[0].text = (i + 1).ToString(); // Rank (1, 2, 3, ...)
            texts[1].text = entry.username;    // Username
            texts[2].text = entry.score.ToString(); // Score
        }
    }
}

// Helper class to store leaderboard entries
public class LeaderboardEntry
{
    public string userId;
    public string username;
    public string email;
    public int score;

    public LeaderboardEntry(string userId, string username, string email, int score)
    {
        this.userId = userId;
        this.username = username;
        this.email = email;
        this.score = score;
    }
}


