using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileItem))]
public class PlayerScanner : MonoBehaviour {

    public AudioClip startScanSound;
    public AudioClip finishScanSound;
    public AudioClip cantScanSound;

    private AudioSource player;

    public float scanCompletionSeconds = 2;
    public PlusOneText plusOneText;
    public Vector3 plusOneTextOffset;

    private PlayableFurnitureItem currentlyScanning;
    private float scanStartTime;

	// Use this for initialization
	void Start () {
        player = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            PlayableFurnitureItem toScan = findItemToScan();

            if (toScan == null) {
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
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

    private PlayableFurnitureItem findItemToScan() {
        TileItem ti = gameObject.GetComponent<TileItem>();

        List<PlayableFurnitureItem> potentialScans = new List<PlayableFurnitureItem>();
        for (int x = ti.tileX - 1; x <= ti.tileX + 1; x++) {
            for (int y = ti.tileY - 1; y <= ti.tileY + 1; y++) {
                foreach (PlayableFurnitureItem item in TileItem.GetObjectsAtPosition<PlayableFurnitureItem>(x, y)) {
                    if (!item.hasBeenScanned && item.gameObject != GetComponent<PlayerTransformer>().getTransformation()) {
                        potentialScans.Add(item);
                    }
                }
            }
        }

        if (potentialScans.Count == 0) {
            return null;
        } else if (potentialScans.Contains(currentlyScanning)) {
            return currentlyScanning;
        } else {
            // TODO - intelligently pick a good thing to scan
            return potentialScans[0];
        }
    }
}
