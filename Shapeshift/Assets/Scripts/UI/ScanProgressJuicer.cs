using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScanProgressJuicer : MonoBehaviour {
    public float JuicePerSecond;
    public GameObject JuicePrefab;

    private CardPanel _panel;
    private PlayerScanner _scanner;
    private float lastJuiceTime;

    private Button _lastScannedButton;
    private List<Juice> _juices;

	// Use this for initialization
	void Start () {
        _juices = new List<Juice>();
        _panel = FindObjectOfType<CardPanel>();
        _scanner = FindObjectOfType<PlayerScanner>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_panel.currentlyScanningButton != _lastScannedButton) {
            foreach (Juice juice in _juices) {
                juice.fadeOut();
            }

            _juices.Clear();
        }

        if (_panel.currentlyScanningButton == null)
        {
            // abort juicing if there's nothing to juice
            lastJuiceTime = Time.time;
            return;
        }

        // compute the quantity of juice to juice
        float delta = Time.time - lastJuiceTime;
        int numJuices = (int)Mathf.Floor(JuicePerSecond * delta);
        if (numJuices == 0) return;

        lastJuiceTime += numJuices / JuicePerSecond;

        // do the juice
        for (int i = 0; i < numJuices; i++)
        {
            makeJuice();
        }

        _lastScannedButton = _panel.currentlyScanningButton;
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
        juice.GetComponent<Juice>().FurnitureImage = _panel.currentlyScanningButton.transform.FindChild("ImageParent").GetChild(0).GetComponent<Image>();
        juice.transform.position = fromPosition;
        juice.transform.SetParent(this.transform);

        _juices.Add(juice.GetComponent<Juice>());
    }
}
