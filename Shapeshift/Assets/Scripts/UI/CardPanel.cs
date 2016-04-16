using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardPanel : MonoBehaviour {

    public GameObject cardPrefab;
    public PlayableFurnitureItem[] furnitureTypes;

	// Use this for initialization
	void Start () {
	    foreach (PlayableFurnitureItem type in furnitureTypes) {
            GameObject button = Instantiate(cardPrefab);
            button.transform.FindChild("Image").GetComponent<Image>().sprite = type.gameObject.GetComponent<FurnitureItem>().image;
            button.transform.FindChild("Name").GetComponent<Text>().text = type.furnitureName;
            button.transform.SetParent(this.transform);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
