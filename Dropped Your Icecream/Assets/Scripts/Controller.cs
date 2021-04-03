using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameManager game;

    public GameObject body = null;    
    public GameObject scoopParent = null;
    public float moveSpeed = 5.0f;

    private void Update() {
        // Movement
        if (Input.GetKey(KeyCode.A)) {
            body.transform.position -= Vector3.right * moveSpeed * Time.deltaTime;
        } 

        if (Input.GetKey(KeyCode.D)) {
            body.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }

    }

    public void Attach(Scoop scoop) {
        
        // Add the scoop to the tower
        scoop.transform.position = this.transform.position;
        scoop.transform.SetParent(scoopParent.transform);
        transform.position += Vector3.up * 0.5f;  
            
        // Update Game Manager
        game.scoopCounter++;
        
        Flavors.ScoreValues.TryGetValue(scoop.Flavor, out int scoreValue);
        game.score += scoreValue; 
        game.activeScoops.Remove(scoop.gameObject);
        
        // Dynamically adjust camera
        if (game.scoopCounter > 5) {      
            Camera.main.transform.position += Vector3.up * 0.5f;
        }
    }

    public void ClearCone() {
        int childs = scoopParent.transform.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            GameObject.DestroyImmediate(scoopParent.transform.GetChild(i).gameObject );
        }
    }
}
