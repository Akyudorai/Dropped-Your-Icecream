using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollectNotif : MonoBehaviour
{
    public Text Display;

    private float decayTimer = 2.5f;
    private void Update() {

        transform.position += Vector3.up * Time.deltaTime;

        if (decayTimer > 0) {
            decayTimer -= Time.deltaTime;
        } else if (decayTimer <= 0) {
            Destroy(this.gameObject);
        }
    }
}
