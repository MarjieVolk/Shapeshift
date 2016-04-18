using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOneTutorialController : MonoBehaviour {

    public CollisionEventCommunicator bathroom;
    public CollisionEventCommunicator goalRoom;

    public TutorialText tutorialTextPrefab;
    public float popupDelayTime = 1.5f;
    
    private bool shiftPopupDisplayed = false;
    private bool shiftPopupDone = false;
    private bool playerHasTransformed = false;

    private float matchRoomPopupTriggeredTime = -1;
    private bool matchRoomPopupDisplayed = false;
    private bool matchRoomPopupDone = false;
    private bool playerInBathroom = false;
    private bool playerHasTransformedIntoSinkInBathroom = false;

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
                playerInBathroom = true;
            }
        };

        bathroom.OnTriggerExit += (GameObject obj) => {
            if (obj.GetComponent<PlayerController>() != null) {
                playerInBathroom = false;
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

        GameObject.FindObjectOfType<PlayerTransformer>().PlayerTransformed += playerTransformedListener;
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
            return playerHasTransformedIntoSinkInBathroom;
        });
    }

    private void createGoalPopup() {
        goalPopupDisplayed = true;

        TutorialText popup = init();
        popup.setText("Pick up the briefcase and escape!");

        FindObjectOfType<VictoryTotem>().OnVictoryTotemAcquired += victoryTotemAquiredListener;

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

    private void playerTransformedListener(GameObject target) {
        playerHasTransformed = true;
        if (playerInBathroom && target.gameObject.GetComponent<PlayableFurnitureItem>().furnitureType == FurnitureType.Sink) {
            playerHasTransformedIntoSinkInBathroom = true;
        }
    }

    private void victoryTotemAquiredListener() {
        playerHasPickedUpBriefcase = true;
    }
}
