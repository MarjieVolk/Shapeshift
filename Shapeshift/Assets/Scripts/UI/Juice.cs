using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UI;

public class Juice : MonoBehaviour {
    public float InitialSpeed;
    public float Acceleration;

    public float minFadeTime;
    public float maxFadeTime;
    public float fadeOutDistance;

    public Vector3 Destination;
    public Image FurnitureImage;

    private Vector2 _velocity;

    private float fadeTime = -1;
    private float startFadeTime = -1;

    private System.Random random;

    // Use this for initialization
    void Start () {
        random = new System.Random();
        GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * InitialSpeed;
        //GetComponent<Image>().sprite = FurnitureImage.sprite;
        int width = FurnitureImage.sprite.texture.width;
        int height = FurnitureImage.sprite.texture.height;
        Color color = new Color(0, 0, 0, 0);
        while (color.a == 0)
        {
            color = FurnitureImage.sprite.texture.GetPixel(
                (int)(FurnitureImage.sprite.texture.width * Random.value),
                (int)(FurnitureImage.sprite.texture.height * Random.value));
        }
        GetComponent<Image>().color = color;
	}
	
	// Update is called once per frame
	void Update () {
        // accelerate towards the destination
        Vector2 towardsDestination = Destination - transform.position;
        if (towardsDestination.magnitude <= fadeOutDistance) {
            fadeOut();
        }

        towardsDestination.Normalize();
        towardsDestination *= Acceleration * Time.deltaTime;
        _velocity += towardsDestination;

        // move the juice
        GetComponent<Rigidbody2D>().velocity += towardsDestination;

        // Fade out
        if (fadeTime != -1) {
            if (Time.time - startFadeTime >= fadeTime) {
                Destroy(this.gameObject);
            } else {
                Color color = this.GetComponent<Image>().color;
                color.a = 1 - (Time.time - startFadeTime) / fadeTime;
                this.GetComponent<Image>().color = color;
            }
        }
	}

    public void fadeOut() {
        if (fadeTime == -1) {
            fadeTime = ((float)random.NextDouble() * (maxFadeTime - minFadeTime)) + minFadeTime;
            startFadeTime = Time.time;
        }
    }
}
