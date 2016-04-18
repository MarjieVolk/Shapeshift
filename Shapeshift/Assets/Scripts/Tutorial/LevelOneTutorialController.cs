using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOneTutorialController : MonoBehaviour {

    public CollisionEventCommunicator bathroom;
    public CollisionEventCommunicator goalRoom;

    public TutorialText tutorialTextPrefab;
    public float popupDelayTime = 3;
    
    private bool shiftPopupDisplayed = false;
    private bool shiftPopupDone = false;
    private bool playerHasTransformed = false;

    private float matchRoomPopupTriggeredTime = -1;
    private bool matchRoomPopupDisplayed = false;
    private bool matchRoomPopupDone = false;

    private bool goalPopupTriggered = false;
    private bool goalPopupDisplayed = false;
    private bool goalPopupDone = false;
    private bool playerHasPickedUpBriefcase = false;
    
    private Canvas canvas;

	// Use this for initialization
	void Start () {
        canvas = GameObject.FindObjectOfType<Canvas>();

        goalRoom.OnTriggerEnter += (GameObject obj) => {
            if (obj.GetComponent<PlayerController>() != null) {
                goalPopupTriggered = true;
            }
        };

        bathroom.OnTriggerEnter += (GameObject obj) => {
            if (obj.GetComponent<PlayerController>() != null) {
                matchRoomPopupTriggeredTime = Time.time;
            }
        };
    }
	
	// Update is called once per frame
	void Update () {
	    if (!shiftPopupDisplayed
            && Time.time >= popupDelayTime) {
            createShiftPopup();
        }

        if (shiftPopupDone
            && !matchRoomPopupDisplayed
            && matchRoomPopupTriggeredTime != -1 
            && Time.time - matchRoomPopupTriggeredTime >= popupDelayTime) {
            createMatchRoomPopup();
        }

        if (matchRoomPopupDone
            && !goalPopupDisplayed
            && goalPopupTriggered) {
            createGoalPopup();
        }
	}

    private void createShiftPopup() {
        shiftPopupDisplayed = true;

        TutorialText shiftPopup = init();
        shiftPopup.setText("To shift your shape, click one of the buttons on your toolbar.");
        shiftPopup.addCloseCondition(() => {
            return playerHasTransformed;
        });
        shiftPopup.OnClose += () => {
            matchRoomPopupTriggeredTime = Time.time;
            shiftPopupDone = true;
        };

        GameObject.FindObjectOfType<PlayerTransformer>().PlayerTransformed += (GameObject target) => {
            playerHasTransformed = true;
        };
    }

    private void createMatchRoomPopup() {
        matchRoomPopupDisplayed = true;

        TutorialText popup = init();
        popup.setText("To hide from guards, transform into a piece of furniture that matches the rest of the room.  "
            + "For a bathroom, try a sink.");

        popup.OnClose += () => {
            matchRoomPopupDone = true;
        };

        popup.addCloseCondition(() => {
            return false;
        });
    }

    private void createGoalPopup() {
        goalPopupDisplayed = true;

        TutorialText popup = init();
        popup.setText("Pick up the briefcase and escape!");

        popup.addCloseCondition(() => {
            return playerHasPickedUpBriefcase;
        });

        popup.OnClose += () => {
            goalPopupDone = true;
        };
    }

    private TutorialText init() {
        TutorialText popup = Instantiate(tutorialTextPrefab.gameObject).GetComponent<TutorialText>();
        popup.transform.SetParent(canvas.transform);
        popup.transform.position += new Vector3(Screen.width / 4, Screen.height / 2);
        return popup;
    }

}
