using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TileItemHighlighter : MonoBehaviour {

    public static TileItemHighlighter INSTANCE;

    public Image highlightPrefab;
    public float fadeOutTime;
    public float borderWidth;

    private Dictionary<TileItem, Image> activeHighlights;
    private List<Image> inactiveHighlightPool;
    private List<TileItem> fadingOut;

    // Use this for initialization
    void Start () {
        INSTANCE = this;

        activeHighlights = new Dictionary<TileItem, Image>();
        inactiveHighlightPool = new List<Image>();
        fadingOut = new List<TileItem>();
	}
	
	// Update is called once per frame
	void Update () {
        List<TileItem> toRemove = new List<TileItem>();
	    foreach (TileItem item in fadingOut) {
            if (!activeHighlights.ContainsKey(item)) {
                toRemove.Add(item);
            } else {
                Color color = activeHighlights[item].color;
                color.a -= Time.deltaTime / fadeOutTime;

                if (color.a <= 0) {
                    color.a = 0;
                    toRemove.Add(item);
                }

                activeHighlights[item].color = color;
            }
        }

        foreach (TileItem item in toRemove) {
            fadingOut.Remove(item);
            clearHighlight(item);
        }
	}

    public void highlight(TileItem item, Color color) {
        fadingOut.Remove(item);

        Image highlight = getImage(item);
        highlight.transform.position = new Vector3(
            item.transform.position.x - borderWidth, 
            item.transform.position.y - borderWidth, 
            Room.ROOM_TILE_Z_INDEX - 1);
        highlight.rectTransform.sizeDelta = new Vector2(
            (item.tileW * TileItem.TILE_SIZE) + (2 * borderWidth) - item.gridOffset.x, 
            (item.tileH * TileItem.TILE_SIZE) + (2 * borderWidth) - item.gridOffset.y);
        highlight.color = color;
    }

    public void clearHighlight(TileItem item) {
        if (activeHighlights.ContainsKey(item)) {
            Image highlight = activeHighlights[item];
            activeHighlights.Remove(item);
            highlight.enabled = false;
            inactiveHighlightPool.Add(highlight);
        }
    }

    public void fadeOutHighlight(TileItem item) {
        if (activeHighlights.ContainsKey(item)) {
            fadingOut.Add(item);
        }
    }

    private Image getImage(TileItem item) {
        Image highlight;

        if (activeHighlights.ContainsKey(item)) {
            highlight = activeHighlights[item];
        } else if (inactiveHighlightPool.Count > 0) {
            highlight = inactiveHighlightPool[0];
            highlight.enabled = true;
            inactiveHighlightPool.RemoveAt(0);
        } else {
            highlight = Instantiate(highlightPrefab);
            highlight.transform.SetParent(this.transform, false);
        }

        activeHighlights[item] = highlight;
        return highlight;
    }
}
