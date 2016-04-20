using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class CardPanel : MonoBehaviour {

    public AudioClip transformSound;
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

        foreach (FurnitureType type in Enum.GetValues(typeof(FurnitureType))) {
            if (UnlockState.INSTANCE.getQualityLevel(type) > -1) {
                // TODO: render quality level and partial progress to next quality level
                add(type);
            } else if (UnlockState.INSTANCE.getScansAboveQualityLevel(type) > 0) {
                addPartial(type);
            }
        }
    }

    private void clear() {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void add(FurnitureType type) {
        Button button = createButton(type, UnlockState.INSTANCE.getQualityLevel(type), "");
        button.interactable = !UnlockState.INSTANCE.isTemporarilyLocked(type);
    }

    private void addPartial(FurnitureType type) {
        int scans = UnlockState.INSTANCE.getScansAboveQualityLevel(type);
        string extraText = "" + scans + "/" + UnlockState.INSTANCE.nScansPerUnlock;
        Button button = createButton(type, 0, extraText);
        button.interactable = false;
    }

    private Button createButton(FurnitureType type, int currentQuality, string extraText) {
        GameObject buttonObj = Instantiate(cardPrefab);
        buttonObj.transform.FindChild("Image").GetComponent<Image>().sprite = FurnitureRenderer.INSTANCE.getSprite(type, currentQuality);
        buttonObj.transform.FindChild("Name").GetComponent<Text>().text = 
            FurnitureRenderer.INSTANCE.getDisplayString(type, currentQuality) + " " + extraText;
        buttonObj.transform.SetParent(this.transform);

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(() => {
            FindObjectOfType<PlayerTransformer>().TransformPlayer(FurnitureRenderer.INSTANCE.getPrefab(type, currentQuality).gameObject);
            GetComponent<AudioSource>().PlayOneShot(transformSound);
        });

        return button;
    }
}
