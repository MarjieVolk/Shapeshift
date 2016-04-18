using UnityEngine;
using System;

[RequireComponent (typeof (FurnitureItem))]
public class PlayableFurnitureItem : MonoBehaviour {

    public FurnitureType furnitureType;
    public string furnitureName;
    public int quality = 0;

    public bool hasBeenScanned = false;

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
