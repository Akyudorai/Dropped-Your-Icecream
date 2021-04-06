using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject body = null;    
    public GameObject scoopParent = null;
    public float moveSpeed = 5.0f;

    public Scoop topScoop = null;
    public List<GameObject> scoopTower = new List<GameObject>();

    public Rigidbody2D rigid2D;

    private void Start() {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void Update() {

        // Pause controls when game is done
        if (GameManager.GetInstance().roundEnd) {
            return;
        }

        // Movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            rigid2D.velocity = -(Vector3.right * moveSpeed);
            //body.transform.position -= Vector3.right * moveSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            rigid2D.velocity = (Vector3.right * moveSpeed);
            //body.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        } else {
            rigid2D.velocity = Vector3.zero;
        }

        // Dynamically adjust camera height 
        if (GameManager.GetInstance().scoopCounter > 5) {
            Vector3 newPosition = new Vector3(0, 1, -10) + new Vector3(0, (1 + (GameManager.GetInstance().scoopCounter - 5)) * 0.5f, 0);
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newPosition, Time.deltaTime);
        } else {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(0, 1, -10), 5*Time.deltaTime);
        }
    }

    public void Attach(Scoop scoop) {

        GameManager game = GameManager.GetInstance();
        // Add the scoop to the tower
        if (topScoop != null) {
            topScoop.isTopScoop = false;
        }
        topScoop = scoop;
        topScoop.isTopScoop = true;
        scoopTower.Add(scoop.gameObject);        
        scoop.transform.SetParent(scoopParent.transform);
            
        // Configure Hinge Joint
        HingeJoint2D joint = scoop.GetComponent<HingeJoint2D>();
        if (game.scoopCounter == 0) {
            scoop.transform.position = this.transform.position;
            joint.useLimits = true;            
            joint.connectedBody = this.GetComponent<Rigidbody2D>();
        } else {
            scoop.transform.position = scoopTower[game.scoopCounter - 1].transform.position + (scoopTower[game.scoopCounter-1].transform.up * 0.5f);
            joint.useLimits = true;
            joint.connectedBody = scoopTower[game.scoopCounter - 1].GetComponent<Rigidbody2D>();
        }
        
        
        // Update Game Manager
       
        game.scoopCounter++;
        
        Flavors.ScoreValues.TryGetValue(scoop.Flavor, out int scoreValue);
        game.score += scoreValue; 
        game.activeScoops.Remove(scoop.gameObject);
        
        
    }

    public void BreakJoint(Scoop scoop) {
        if (!scoopTower.Contains(scoop.gameObject)) {
            return;
        }

        int index = scoopTower.IndexOf(scoop.gameObject);

        // If the bottom most scoop falls, reset the cone and score
        if (index == 0) {
            ClearCone(false);
            GameManager.GetInstance().score = 0;
            GameManager.GetInstance().scoopCounter = 0;
            return;
        }

        // Otherwise, break the chain and adjust game values accordingly (points lost, scoops remaining on cone, etc)
        topScoop.isTopScoop = false;
        topScoop = scoopTower[index-1].GetComponent<Scoop>();
        topScoop.isTopScoop = true;
        for (int i = scoopTower.Count - 1; i >= index; i--) {            
            Flavors.ScoreValues.TryGetValue(scoopTower[i].GetComponent<Scoop>().Flavor, out int scoreValue);
            GameManager.GetInstance().score -= scoreValue;
            GameManager.GetInstance().scoopCounter--;
            scoopTower.RemoveAt(i);            
        }        
    }

    public void ClearCone(bool destroyObjects) {
        if (destroyObjects) {
            int childs = scoopParent.transform.childCount;
            for (int i = childs - 1; i >= 0; i--) {
                GameObject.DestroyImmediate(scoopParent.transform.GetChild(i).gameObject );
            }
        }
        

        scoopTower.Clear();
        scoopTower = new List<GameObject>();
    }
}
