using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum IcecreamTypes { 
    Vanilla, // 25pt
    Chocolate, // 50pts
    Strawberry, // 65 pts
    Neapolitain, // 100 pts
    
    PeanutButterChunk, // 150pts
    CookieDough, // 200 pts
    CookiesNCream, // 400 pts
    MintChocoChip, // 600 pts
    
    Birthday, // 1000 pts
    FishFace  // -300 pts
}

public static class Flavors {     

    public static Dictionary<string, Color> ColorValues = new Dictionary<string, Color>();
    public static Dictionary<string, int> ScoreValues = new Dictionary<string, int>(); 

    public static void Initialize() {
        ScoreValues.Add("Vanilla", 25);  
        ColorValues.Add("Vanilla", new Color(1f, 1f, 1f, 1f));

        ScoreValues.Add("Chocolate", 50);
        ColorValues.Add("Chocolate", new Color(0.388f, 0.282f, 0.215f, 1f));

        ScoreValues.Add("Strawberry", 75);
        ColorValues.Add("Strawberry", new Color(0.984f, 0.588f, 0.811f, 1f));

        // Change to white/brown/pink gradient
        ScoreValues.Add("Neapolitain", 100);
        ColorValues.Add("Neapolitain", new Color(0.858f, 0.137f, 0f, 1f));

        // Add dotted "chunks"
        ScoreValues.Add("PeanutButterChunk", 150);
        ColorValues.Add("PeanutButterChunk", new Color(0.890f, 0.666f, 0.447f, 1f));
    
        ScoreValues.Add("CookieDough", 200);
        ColorValues.Add("CookieDough", new Color(0.956f, 0.874f, 0.603f, 1f));

        // Add dotted black'n'white "oreo"         
        ScoreValues.Add("CookiesNCream", 400);
        ColorValues.Add("CookiesNCream", new Color(0.854f, 0.850f, 0.807f, 1f));
        
        // Add dotted cookie pieces
        ScoreValues.Add("MintChocoChip", 600);
        ColorValues.Add("MintChocoChip", new Color(0.494f, 0.929f, 0.701f, 1f));
        
        // All the colors :D
        ScoreValues.Add("Birthday", 1000);
        ColorValues.Add("Birthday", new Color(0.803f, 0f, 0.858f, 1f));
        
        // Gross
        ScoreValues.Add("FishFace", -300);
        ColorValues.Add("FishFace", new Color(0.494f, 0.929f, 0.894f, 1f));
    }
}

public class Scoop : MonoBehaviour
{
    public string Flavor = "Vanilla";
    private bool isFalling = true;
    private void Update() {

        // Fall over time
        if (isFalling) {
            transform.position -= Vector3.up * Time.deltaTime;
        }
        
    }

    public void SetFlavor(int scoopType) {
        Flavor = Enum.GetName(typeof(IcecreamTypes), scoopType);        
        Flavors.ColorValues.TryGetValue(Flavor, out Color flavorColor);
        GetComponent<SpriteRenderer>().color = flavorColor;

        // If it's the fish face, replace the sprite
        if (Flavor == "FishFace") {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fish");
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        
        if (col.tag == "Player") {            
            isFalling = false;
            col.GetComponent<Controller>().Attach(this);
        }

        if (col.tag == "Floor") {
            // Don't count fish as a missed scoop.
            if (Flavor != "FishFace") {
                // Change to Singleton format
                GameObject.Find("GameManager").GetComponent<GameManager>().missedScoops++;
            }            
            // Drop a splat at the position of contact.            
            Destroy(gameObject);
        }
    }

}
