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

    private PlayableFurnitureItem _lastScanned;
    private List<Juice> _juices;

	// Use this for initialization
	void Start () {
        _juices = new List<Juice>();
        _panel = FindObjectOfType<CardPanel>();
        _scanner = FindObjectOfType<PlayerScanner>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_scanner.currentlyScanning != _lastScanned) {
            foreach (Juice juice in _juices) {
                juice.fadeOut();
            }

            _juices.Clear();
        }

        if (_panel.currentlyScanningButtonPosition == null || _scanner.currentlyScanning == null)
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

        _lastScanned = _scanner.currentlyScanning;
    }

    /// <summary>
    /// Makes a juice.
    /// </summary>
    private void makeJuice()
    {
        Vector2 fromPosition = Camera.main.WorldToScreenPoint(_scanner.currentlyScanning.GetComponent<TileItem>().getCenterPosition());
        Vector2 toPosition = _panel.currentlyScanningButtonPosition;

        GameObject juice = Instantiate<GameObject>(JuicePrefab);
        juice.GetComponent<Juice>().Destination = toPosition;
        juice.GetComponent<Juice>().FurnitureImage = _scanner.currentlyScanning.GetComponent<SpriteRenderer>().sprite.texture;
        juice.transform.position = fromPosition;
        juice.transform.SetParent(this.transform);

        _juices.Add(juice.GetComponent<Juice>());
    }
}
