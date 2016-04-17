using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureRenderer : MonoBehaviour {

    public static FurnitureRenderer INSTANCE;

    public PlayableFurnitureItem[] allPossibleFurniture;

    private Dictionary<FurnitureKey, PlayableFurnitureItem> furniture;

    void Awake() {
        INSTANCE = this;

        furniture = new Dictionary<FurnitureKey, PlayableFurnitureItem>();
        foreach (PlayableFurnitureItem item in allPossibleFurniture) {
            FurnitureKey key = new FurnitureKey();
            key.type = item.furnitureType;
            key.quality = item.quality;
            furniture.Add(key, item);
        }
    }
	
	public string getDisplayString(FurnitureType type, int quality) {
        return getData(type, quality).furnitureName;
    }

    public Sprite getSprite(FurnitureType type, int quality) {
        return getData(type, quality).GetComponent<SpriteRenderer>().sprite;
    }

    public PlayableFurnitureItem getPrefab(FurnitureType type, int quality) {
        return getData(type, quality);
    }

    private PlayableFurnitureItem getData(FurnitureType type, int quality) {
        return furniture[FurnitureKey.getKey(type, quality)];
    }

    private string renderKeys() {
        string str = "";

        foreach (FurnitureKey key in furniture.Keys) {
            str += " (" + key.type + ", " + key.quality + ")";
        }

        return str;
    }

}
