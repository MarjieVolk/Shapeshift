using UnityEngine;
using System.Collections;

public class ScanProgressJuicer : MonoBehaviour {
    public float JuicePerSecond;
    public GameObject JuicePrefab;

    private CardPanel _panel;
    private PlayerScanner _scanner;
    private float lastJuiceTime;

	// Use this for initialization
	void Start () {
        _panel = FindObjectOfType<CardPanel>();
        _scanner = FindObjectOfType<PlayerScanner>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_panel.currentlyScanningButton == null)
        {
            // abort juicing if there's nothing to juice
            lastJuiceTime = Time.time;
            return;
        }

        Debug.Log("Juicing...");

        // compute the quantity of juice to juice
        float delta = Time.time - lastJuiceTime;
        Debug.Log(delta + " is delta");
        Debug.Log(lastJuiceTime + " is last juice time.");
        int numJuices = (int)Mathf.Floor(JuicePerSecond * delta);
        if (numJuices == 0) return;

        lastJuiceTime += numJuices / JuicePerSecond;

        Debug.Log(numJuices + " juices!");

        // do the juice
        for (int i = 0; i < numJuices; i++)
        {
            makeJuice();
        }
	}

    /// <summary>
    /// Makes a juice.
    /// </summary>
    private void makeJuice()
    {
        Vector2 fromPosition = Camera.main.WorldToScreenPoint(_scanner.currentlyScanning.transform.position);
        Vector2 toPosition = _panel.currentlyScanningButton.transform.position;

        GameObject juice = Instantiate<GameObject>(JuicePrefab);
        juice.GetComponent<Juice>().Destination = toPosition;
        juice.transform.position = fromPosition;
        juice.transform.SetParent(this.transform);
    }
}
