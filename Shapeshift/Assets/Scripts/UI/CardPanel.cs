﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class CardPanel : MonoBehaviour {

    public AudioClip transformSound;
    public GameObject cardPrefab;
    public Vector3 currentlyScanningButtonPosition;

    private Dictionary<FurnitureType, Button> buttons;

	// Use this for initialization
	void Start () {
        buttons = new Dictionary<FurnitureType, Button>();
        UnlockState.INSTANCE.UnlockStateChanged += refresh;
        refresh();
	}

    void OnDestroy() {
        UnlockState.INSTANCE.UnlockStateChanged -= refresh;
    }
	
    public void refresh() {
        foreach (FurnitureType type in Enum.GetValues(typeof(FurnitureType))) {
            float scans = UnlockState.INSTANCE.getScansAboveQualityLevel(type);
            // If it has a fractional scan (i.e. it is currently being scanned)
            if (Mathf.Abs(scans % 1) > 0.001f && buttons.ContainsKey(type)) {
                currentlyScanningButtonPosition = buttons[type].transform.position;
            }
        }

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

        buttons.Clear();
    }

    private void add(FurnitureType type) {
        Button button = createButton(type, UnlockState.INSTANCE.getQualityLevel(type), "");
        button.interactable = !UnlockState.INSTANCE.isTemporarilyLocked(type);
    }

    private void addPartial(FurnitureType type) {
        float scans = UnlockState.INSTANCE.getScansAboveQualityLevel(type);
        string formatString = "0";
        string extraText = "" + scans.ToString(formatString) + "/" + UnlockState.INSTANCE.nScansPerUnlock;
        Button button = createButton(type, 0, extraText, scans / UnlockState.INSTANCE.nScansPerUnlock);
        button.interactable = false;
    }

    private Button createButton(FurnitureType type, int currentQuality, string extraText, float partialProgress = 1) {
        GameObject buttonObj = Instantiate(cardPrefab);

        Image image = buttonObj.transform.FindChild("ImageParent").FindChild("Image").GetComponent<Image>();
        image.sprite = FurnitureRenderer.INSTANCE.getSprite(type, currentQuality);
        image.type = Image.Type.Filled;
        image.fillAmount = partialProgress;
        image.fillMethod = Image.FillMethod.Vertical;

        buttonObj.transform.FindChild("Name").GetComponent<Text>().text = 
            FurnitureRenderer.INSTANCE.getDisplayString(type, currentQuality) + " " + extraText;
        buttonObj.transform.SetParent(this.transform);

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(() => {
            FindObjectOfType<PlayerTransformer>().TransformPlayer(FurnitureRenderer.INSTANCE.getPrefab(type, currentQuality).gameObject);
            GetComponent<AudioSource>().PlayOneShot(transformSound);
        });

        buttons.Add(type, button);
        return button;
    }
}
