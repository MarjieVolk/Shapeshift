using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileItem))]
[RequireComponent(typeof(AudioSource))]
public class PlayerScanner : MonoBehaviour {

    public AudioClip startScanSound;
    public AudioClip finishScanSound;
    public AudioClip cantScanSound;

    private AudioSource player;

    public float scanCompletionSeconds = 2;
    public PlusOneText plusOneText;
    public Vector3 plusOneTextOffset;

    public Color activeColor;
    public Color potentialColor;

    private PlayableFurnitureItem currentlyScanning;
    private float scanStartTime;

    private List<PlayableFurnitureItem> shownHighlights;

    // Use this for initialization
    void Start () {
        player = gameObject.GetComponent<AudioSource>();
        shownHighlights = new List<PlayableFurnitureItem>();
    }
	
	// Update is called once per frame
	void Update () {
        List<PlayableFurnitureItem> potentialScans;
        PlayableFurnitureItem toScan = findItemToScan(out potentialScans);

        clearHighlights();
        foreach (PlayableFurnitureItem item in potentialScans) {
            highlight(item, potentialColor);
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (toScan != null) {
                highlight(toScan, activeColor);
            }

            if (toScan == null) {
                currentlyScanning = null;
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
                    // Tried to scan and can't
                    player.PlayOneShot(cantScanSound);
                }

                if (currentlyScanning != null)
                {
                    UnlockState.INSTANCE.abandonProgressOn(currentlyScanning.furnitureType);
                    currentlyScanning = null;
                }
            } else if (currentlyScanning == toScan) {
                // Continue scanning
                UnlockState.INSTANCE.registerProgressOn(currentlyScanning.furnitureType, Time.deltaTime / scanCompletionSeconds);
                if (scanStartTime != -1 && Time.time - scanStartTime >= scanCompletionSeconds) {
                    // Finish scanning
                    if (!currentlyScanning.hasBeenScanned) {
                        currentlyScanning.hasBeenScanned = true;
                        UnlockState.INSTANCE.completeScanOn(currentlyScanning.furnitureType);
                        spawnPlusOneText(currentlyScanning.furnitureType);

                        player.PlayOneShot(finishScanSound);
                    }
                    currentlyScanning = null;
                }
            } else {
                if (currentlyScanning != null)
                {
                    UnlockState.INSTANCE.abandonProgressOn(currentlyScanning.furnitureType);
                }
                // Start new scan
                currentlyScanning = toScan;
                scanStartTime = Time.time;

                player.PlayOneShot(startScanSound);
            }
        } else {
            currentlyScanning = null;
            scanStartTime = -1;
            if (currentlyScanning != null)
            {
                Debug.Log("Scan not pressed.");
                UnlockState.INSTANCE.abandonProgressOn(currentlyScanning.furnitureType);
                currentlyScanning = null;
            }
        }
    }

    private void spawnPlusOneText(FurnitureType type) {
        PlusOneText text = Instantiate(plusOneText);
        text.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        text.setFurnitureType(type);
        text.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + plusOneTextOffset);
    }

    private void clearHighlights() {
        foreach (PlayableFurnitureItem item in shownHighlights) {
            TileItemHighlighter.INSTANCE.clearHighlight(item.GetComponent<TileItem>());
        }

        shownHighlights.Clear();
    }

    private void highlight(PlayableFurnitureItem item, Color color) {
        TileItemHighlighter.INSTANCE.highlight(item.GetComponent<TileItem>(), color);
        shownHighlights.Add(item);
    }

    private PlayableFurnitureItem findItemToScan(out List<PlayableFurnitureItem> potentialScans) {
        getPotentialScans(out potentialScans);

        if (potentialScans.Count == 0) {
            return null;
        } else if (potentialScans.Contains(currentlyScanning)) {
            return currentlyScanning;
        } else {
            // TODO - intelligently pick a good thing to scan
            return potentialScans[0];
        }
    }

    private void getPotentialScans(out List<PlayableFurnitureItem> potentialScans) {
        potentialScans = new List<PlayableFurnitureItem>();

        foreach (PlayableFurnitureItem item in getFurnitureInRange()) {
            if (item.gameObject != GetComponent<PlayerTransformer>().getTransformation()) {
                if (!item.hasBeenScanned) {
                    potentialScans.Add(item);
                }
            }
        }
    }

    private List<PlayableFurnitureItem> getFurnitureInRange() {
        TileItem ti = gameObject.GetComponent<TileItem>();
        List<PlayableFurnitureItem> furniture = new List<PlayableFurnitureItem>();

        for (int x = ti.tileX - 1; x <= ti.tileX + 1; x++) {
            for (int y = ti.tileY - 1; y <= ti.tileY + 1; y++) {
                furniture.AddRange(TileItem.GetObjectsAtPosition<PlayableFurnitureItem>(x, y));
            }
        }

        return furniture;
    }
}
