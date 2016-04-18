using UnityEngine;
using System.Collections;

public class LevelOneTutorialController : MonoBehaviour {

    public Collider2D bathroom;

    public TutorialText tutorialTextPrefab;
    public float popupDelayTime = 3;

    private bool shiftPopupDone = false;
    private bool playerHasTransformed = false;

    private float matchRoomPopupTriggeredTime = -1;
    private bool matchRoomPopupDone = false;

    private Canvas canvas;

	// Use this for initialization
	void Start () {
        canvas = GameObject.FindObjectOfType<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
        bathroom.on
	    if (!shiftPopupDone && Time.time >= popupDelayTime) {
            createShiftPopup();
        }

        if (!matchRoomPopupDone && matchRoomPopupTriggeredTime != -1 && Time.time - matchRoomPopupTriggeredTime >= popupDelayTime) {
            createMatchRoomPopup();
        }
	}

    private void createShiftPopup() {
        shiftPopupDone = true;

        TutorialText shiftPopup = init();
        shiftPopup.setText("To shift your shape, click one of the buttons on your toolbar.");
        shiftPopup.addCloseCondition(() => {
            return playerHasTransformed;
        });
        shiftPopup.addCloseHandler(() => {
            matchRoomPopupTriggeredTime = Time.time;
        });

        GameObject.FindObjectOfType<PlayerTransformer>().PlayerTransformed += (GameObject target) => {
            playerHasTransformed = true;
        };
    }

    private void createMatchRoomPopup() {
        matchRoomPopupDone = true;

        TutorialText popup = init();
        popup.setText("To hide from guards, transform into a piece of furniture that matches the rest of the room.  "
            + "For a bathroom, try a sink.");
    }

    private TutorialText init() {
        TutorialText popup = Instantiate(tutorialTextPrefab.gameObject).GetComponent<TutorialText>();
        popup.transform.SetParent(canvas.transform);
        popup.transform.position += new Vector3(Screen.width / 4, Screen.height / 2);
        return popup;
    }
}
