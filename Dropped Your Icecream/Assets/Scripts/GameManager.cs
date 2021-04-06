using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /* Rules of the game
    - This game is a stacking game.  When the player starts, scoops of icecream will drop from the top of the screen towards the bottom of the screen slowly.
    - At the bottom of the screen is an icecream cone / bowl to collect the icecream scoops that are falling.  
    - The player can move the icecream cone/bowl with touchscreen / keyboard controls / mouse to catch the falling scoops.
    - As the player catches more scoops of icecream, a stack will be formed. The higher your stack of ice cream at the end of the round (timer) the better your score.
    - If you match certain flavors of icecream together, you get bonus points!
    
    // Cute tidbits
    - The larger your stack, the more flexy/bendable your icecream tower will become.
    - i.e. if you move your cone to the right, their will be a little bit of a delay between the start of the cone and the top of the tower.
    - The player must land the icecream on the top of the icecream cone to collect a scoop. 
    - if it lands on the side of the cone, it'll knock off some ice cream and fall to the floor, reducing your stack.
    
    - Every time you drop an icecream scoop, it creates a splatter effect on the bottom of the screen, below the cone/bowl. 
    - This could possibly get pretty artsy, collectively. 
    - After all, half the fun is the mess right? 

    - Future additions: 
    -- Multiplayer face-off: Stack Attack! (Snack Attack!)
    */

    // Singleton Format
    private static GameManager instance;
    public static GameManager GetInstance() { 
        return instance;
    }

    // Game Variables
    private int difficulty;
    public Controller controller; 
    public float RoundDuration = 60.0f;
    private float roundTimer;
    public int scoopCounter = 0;
    public int missedScoops = 0;
    public int score = 0;
    public bool lastScoop = false;
    public bool roundEnd = false;
    private float scoopSpawnTime = 3.0f;

    public List<GameObject> activeScoops;

    // UI Variables
    public GameObject gameInfoPanel;
    public Text scoreText;
    public Text timeText;
    public Text counterText;
    public Text finalScoopText;

    public GameObject endScreenPanel;
    public Text endScoreText;
    public Text endMissedText;
    public Text endCaughtText;
    

    private void Awake() {
        instance = this;
    }

    private void Start() {        
        Flavors.Initialize();
        NewGame(0);
    }

    private void Update() {
        
        UpdateUI();
        UpdateList();


        // Timer
        if (roundTimer > 0) {
            roundTimer -= Time.deltaTime;
        } else if (roundTimer <= 0 && !lastScoop) {
            StartCoroutine(LastScoopNotification());            
        }

        // End Check
        if (roundTimer <= 0 && !roundEnd && activeScoops.Count == 0) {
            RoundEnd();
        }
    }

    private void UpdateList() {
        for(var i = activeScoops.Count - 1; i > -1; i--) {
            if (activeScoops[i] == null) {
                activeScoops.RemoveAt(i);
            }
            
        }
    }

    public void NewGame(int difficulty) {

        this.difficulty = difficulty;
        // Change scoop patters, time, difficulty, etc. based on level index        
        /// Example: Spawn Time of Scoops.  Gradually increases spawn speed, with a limiter of scoops per second.         
        scoopSpawnTime = 3.0f - (difficulty * 0.25f);
        if (scoopSpawnTime < 0.25f) {
            scoopSpawnTime = 0.25f;
        } 

        activeScoops = new List<GameObject>();
        lastScoop = false;
        roundEnd = false;
        score = 0;
        scoopCounter = 0;
        missedScoops = 0;
        roundTimer = RoundDuration;        
        gameInfoPanel.SetActive(true);
        endScreenPanel.SetActive(false);
        controller.ClearCone(true);
        StartCoroutine(SpawnScoop());
    }

    private IEnumerator SpawnScoop() {        

        while (roundTimer > 0) {
            yield return new WaitForSeconds(scoopSpawnTime);

            // Change the spawning system from random to level based using JSON
            int scoopType = Random.Range(0, 10);

            if (scoopCounter > 5) {
                GameObject scoopObject = Instantiate(Resources.Load<GameObject>("Prefabs/Scoop"), new Vector2(Random.Range(-5, 5), 7 + (scoopCounter-5)), Quaternion.identity);
                scoopObject.GetComponent<Scoop>().SetFlavor(scoopType);
                activeScoops.Add(scoopObject);
            } else {
                GameObject scoopObject = Instantiate(Resources.Load<GameObject>("Prefabs/Scoop"), new Vector2(Random.Range(-5, 5), 7), Quaternion.identity);
                scoopObject.GetComponent<Scoop>().SetFlavor(scoopType);
                activeScoops.Add(scoopObject);
            }
                    
        }        
    }



    private IEnumerator LastScoopNotification() {
        lastScoop = true;
        finalScoopText.enabled = true;
        yield return new WaitForSeconds(2.0f);
        finalScoopText.enabled = false;
    }



    private void UpdateUI() {
        timeText.text = "Time: " + Mathf.RoundToInt(roundTimer);
        scoreText.text = "Score: " + score;
        counterText.text = "Scoops: " + scoopCounter;        
    }

    private void RoundEnd() {
        gameInfoPanel.SetActive(false);
        endScreenPanel.SetActive(true);
        endScoreText.text = "Final Score: " + score;
        endCaughtText.text = "Scoops Caught: " + scoopCounter;
        endMissedText.text = "Scoops Missed: " + missedScoops;
    }

    public void NextLevel() {        
        NewGame(difficulty + 1);
    }

    public void RetryLevel() {
        NewGame(difficulty);
    }
}
