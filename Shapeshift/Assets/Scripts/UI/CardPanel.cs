using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardPanel : MonoBehaviour {

    public GameObject cardPrefab;
    public PlayableFurnitureItem[] startingFurnitureTypes;

    private Dictionary<string, GameObject> buttons;

	// Use this for initialization
	void Start () {
        buttons = new Dictionary<string, GameObject>();

	    foreach (PlayableFurnitureItem type in startingFurnitureTypes) {
            add(type);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void remove(PlayableFurnitureItem item) {
        if (!buttons.ContainsKey(item.furnitureName)) {
            return;
        }

        GameObject toRemove = buttons[item.furnitureName];
        buttons.Remove(item.furnitureName);
        toRemove.transform.SetParent(null);
        Destroy(toRemove);
    }

    public void add(PlayableFurnitureItem item) {
        if (buttons.ContainsKey(item.furnitureName)) {
            return;
        }

        GameObject button = Instantiate(cardPrefab);
        button.transform.FindChild("Image").GetComponent<Image>().sprite = item.gameObject.GetComponent<SpriteRenderer>().sprite;
        button.transform.FindChild("Name").GetComponent<Text>().text = item.furnitureName;
        button.transform.SetParent(this.transform);

        button.GetComponent<Button>().onClick.AddListener(() => {
            FindObjectOfType<PlayerTransformer>().TransformPlayer(item.gameObject);
        });

        buttons.Add(item.furnitureName, button);
    }
}
