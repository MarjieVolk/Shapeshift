using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardPanel : MonoBehaviour {

    public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
        UnlockState.INSTANCE.UnlockStateChanged += refresh;
        refresh();
	}

    void OnDestroy() {
        UnlockState.INSTANCE.UnlockStateChanged -= refresh;
    }
	
    public void refresh() {
        clear();

        foreach (FurnitureType type in UnlockState.INSTANCE.getUnlocked()) {
            add(type);
        }
    }

    private void clear() {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void add(FurnitureType type) {
        int currentQuality = UnlockState.INSTANCE.getQualityLevel(type);

        GameObject buttonObj = Instantiate(cardPrefab);
        buttonObj.transform.FindChild("Image").GetComponent<Image>().sprite = FurnitureRenderer.INSTANCE.getSprite(type, currentQuality);
        buttonObj.transform.FindChild("Name").GetComponent<Text>().text = FurnitureRenderer.INSTANCE.getDisplayString(type, currentQuality);
        buttonObj.transform.SetParent(this.transform);

        Button button = buttonObj.GetComponent<Button>();
        button.interactable = !UnlockState.INSTANCE.isTemporarilyLocked(type);
        if (UnlockState.INSTANCE.isTemporarilyLocked(type)) {
            Debug.Log("Locking " + type);
        }
        button.onClick.AddListener(() => {
            FindObjectOfType<PlayerTransformer>().TransformPlayer(FurnitureRenderer.INSTANCE.getPrefab(type, currentQuality).gameObject);
        });
    }
}
