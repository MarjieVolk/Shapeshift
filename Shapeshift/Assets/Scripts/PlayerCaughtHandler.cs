using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(PlayerTransformer))]
public class PlayerCaughtHandler : MonoBehaviour {
    
    public event UnityAction PlayerCaught;

    public delegate void PlayerCaughtAsFurnitureHanlder(FurnitureType type);
    public event PlayerCaughtAsFurnitureHanlder PlayerCaughtAsFurniture;

    public event UnityAction PlayerCaughtAsHuman;

    private Vector3 startPosition;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
    }

    public void catchPlayer() {
        GameObject currentTransformation = GetComponent<PlayerTransformer>().getTransformation();
        if (currentTransformation == null) {
            // Transport player to beginning
            gameObject.GetComponent<TileItem>().SetGlobalPosition(startPosition);

            if (PlayerCaughtAsHuman != null) {
                PlayerCaughtAsHuman();
            }
        } else {
            // Lose access to item, gain catching cooldown
            if (PlayerCaughtAsFurniture != null) {
                PlayerCaughtAsFurniture(currentTransformation.GetComponent<PlayableFurnitureItem>().furnitureType);
            }

            GetComponent<PlayerTransformer>().TransformPlayer(null);
        }

        if (PlayerCaught != null) {
            PlayerCaught();
        }
    }
}
