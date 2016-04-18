using System;

public class FurnitureKey {
    public FurnitureType type;
    public int quality;

    public override int GetHashCode() {
        return ((int) type * 8) + quality;
    }

    public override bool Equals(object obj) {
        if (obj.GetType().IsAssignableFrom(typeof(FurnitureKey))) {
            FurnitureKey otherKey = (FurnitureKey) obj;
            return otherKey.type == this.type && otherKey.quality == this.quality;
        } else {
            return false;
        }
    }

    public static FurnitureKey getKey(PlayableFurnitureItem item) {
        FurnitureKey key = new FurnitureKey();
        key.type = item.furnitureType;
        key.quality = item.quality;

        return key;
    }

    public static FurnitureKey getKey(FurnitureType type, int quality) {
        FurnitureKey key = new FurnitureKey();
        key.type = type;
        key.quality = quality;

        return key;
    }
}
