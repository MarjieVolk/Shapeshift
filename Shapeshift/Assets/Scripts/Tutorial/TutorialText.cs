using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TutorialText : MonoBehaviour {

    public delegate bool CloseCondition();

    private List<UnityAction> closeActions;
    private List<CloseCondition> closeConditions;

	// Use this for initialization
	void Awake () {
        closeActions = new List<UnityAction>();
        closeConditions = new List<CloseCondition>();

        transform.FindChild("Close Button").GetComponent<Button>().onClick.AddListener(() => {
            close();
        });
	}

    void Update () {
        foreach (CloseCondition closeCondition in closeConditions) {
            if (closeCondition()) {
                close();
                break;
            }
        }
    }

    public void setText(string text) {
        transform.FindChild("Text").GetComponent<Text>().text = text;
    }

    public void addCloseCondition(CloseCondition condition) {
        closeConditions.Add(condition);
    }

    public void addCloseHandler(UnityAction action) {
        closeActions.Add(action);
    }

    public void close() {
        foreach (UnityAction action in closeActions) {
            action();
        }

        Destroy(gameObject);
    }
}
