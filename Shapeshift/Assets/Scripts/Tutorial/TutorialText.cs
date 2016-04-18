using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TutorialText : MonoBehaviour {

    public delegate bool CloseCondition();

    public event UnityAction OnClose;
    
    private List<CloseCondition> closeConditions;

	// Use this for initialization
	void Awake () {
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

    public void close() {
        OnClose();
        Destroy(gameObject);
    }
}
