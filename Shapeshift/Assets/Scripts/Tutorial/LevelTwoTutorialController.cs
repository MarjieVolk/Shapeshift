using UnityEngine;
using System.Collections;

public class LevelTwoTutorialController : MonoBehaviour {

    public float popupDelayTime = 1.5f;
    public TutorialText tutorialTextPrefab;

    private float scanPopupTriggeredTime;
    private bool scanPopupDisplayed = false;
    private bool playerHasScanned = false;

    private float unlockPopupTriggeredTime = -1;
    private bool unlockPopupDisplayed = false;

    private Canvas canvas;

    // Use this for initialization
    void Start () {
        canvas = GameObject.FindObjectOfType<Canvas>();
        scanPopupTriggeredTime = Time.time;
        UnlockState.INSTANCE.ScanCompleted += handleScanCompleted;
    }

    void OnDestroy() {
        UnlockState.INSTANCE.ScanCompleted -= handleScanCompleted;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!scanPopupDisplayed && Time.time - scanPopupTriggeredTime > popupDelayTime) {
            createScanPopup();
        }

        if (!unlockPopupDisplayed && unlockPopupTriggeredTime != -1) {
            createUnlockPopup();
        }
	}

    private void createScanPopup() {
        scanPopupDisplayed = true;

        TutorialText popup = init();
        popup.setText("Hold shift while next to an item of furniture to scan it.  It will take a few seconds.");

        popup.OnClose += () => {
            unlockPopupTriggeredTime = Time.time;
        };

        popup.addCloseCondition(() => {
            return playerHasScanned;
        });
    }

    private void createUnlockPopup() {
        unlockPopupDisplayed = true;

        TutorialText popup = init();
        popup.setText("Scan three of the same type of furniture to gain the ability to shift into it!  " 
            + "But be careful, if you are caught you will loose all your scans from this level.");

        popup.addCloseCondition(() => {
            return Time.time - unlockPopupTriggeredTime > 5;
        });
    }

    private TutorialText init() {
        TutorialText popup = Instantiate(tutorialTextPrefab.gameObject).GetComponent<TutorialText>();
        popup.transform.SetParent(canvas.transform);
        popup.transform.position += new Vector3(Screen.width / 4, Screen.height / 2);
        return popup;
    }

    private void handleScanCompleted() {
        playerHasScanned = true;
    }
}
