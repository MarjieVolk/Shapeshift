using System;
using System.Collections.Generic;
using UnityEngine;

class Suspicionomatic {

    public static int getSuspicionLevel(FurnitureItem item) {
        int suspicionLevel = 0;

        // +1 suspicion level per item in the same room that does not share a room type
        TileItem itemTileItem = item.GetComponent<TileItem>();
        List<Room> potentialRooms = TileItem.GetObjectsAtPosition<Room>(itemTileItem.tileX, itemTileItem.tileY);

        if (potentialRooms.Count != 1) {
            Debug.LogError("Found " + potentialRooms.Count + " rooms at (" + itemTileItem.tileX + ", " + itemTileItem.tileY + ")");
        }
        
        foreach (FurnitureItem other in potentialRooms[0].getAllFurniture()) {
            if (other != item) {
                bool hasRoomTypeMatch = false;
                foreach (RoomType itemRoomType in item.roomTypes) {
                    foreach (RoomType otherRoomType in other.roomTypes) {
                        if (itemRoomType == otherRoomType) {
                            hasRoomTypeMatch = true;
                            break;
                        }
                    }
                }

                if (!hasRoomTypeMatch) {
                    Debug.Log("" + item + " has no room type match with " + other);
                    suspicionLevel++;
                }
            }
        }

        // + suspicion level for each broken placement rule
        // TODO

        return suspicionLevel;
    }
}
