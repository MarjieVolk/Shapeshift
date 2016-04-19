using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(PlayerTransformer))]
public class PlayerCaughtHandler : MonoBehaviour {

    public AudioClip[] caughtSounds;

    public event UnityAction PlayerCaught;

    public delegate void PlayerCaughtAsFurnitureHanlder(FurnitureType type);
    public event PlayerCaughtAsFurnitureHanlder PlayerCaughtAsFurniture;

    public event UnityAction PlayerCaughtAsHuman;


    public float caughtCooldown = 3;
    public float caughtBlinkPeriod = 0.5f;

    private Vector3 startPosition;
    private float caughtTime = 0;

    private System.Random random;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        random = new System.Random();
    }

    void Update()
    {
        if (isOnCatchCooldown())
        {
            float elapsedTime = Time.time - caughtTime;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (elapsedTime / caughtBlinkPeriod) % 1);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void catchPlayer() {
        caughtTime = Time.time;
        GetComponent<AudioSource>().PlayOneShot(caughtSounds[random.Next(caughtSounds.Length)]);

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

    public bool isOnCatchCooldown() {
        return Time.time - caughtTime < caughtCooldown;
    }
}
