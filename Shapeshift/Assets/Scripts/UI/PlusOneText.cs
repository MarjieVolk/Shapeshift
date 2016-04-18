using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlusOneText : MonoBehaviour {

    public Vector2 floatDirection;
    public float floatSpeed;
    public float fadeOutTime;

    private float initTime;

	// Use this for initialization
	void Start () {
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time - initTime > fadeOutTime) {
            Destroy(this.gameObject);
        } else {
            Vector3 translation = new Vector3(floatDirection.normalized.x, floatDirection.normalized.y, 0) * floatSpeed;
            transform.position += translation;

            GetComponent<CanvasGroup>().alpha = 1f - ((Time.time - initTime) / fadeOutTime);
        }
	}

    public void setFurnitureType(FurnitureType type) {
        transform.FindChild("Furniture Type Text").GetComponent<Text>().text = 
            FurnitureRenderer.INSTANCE.getDisplayString(type, UnlockState.INSTANCE.getQualityLevel(type) + 1);
    }
}
