using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorMenuItems {

    [MenuItem("Shifters/Regenerate floors")]
    private static void regenerateFloors() {
        foreach (Room room in GameObject.FindObjectsOfType<Room>()) {
            room.generateFloorSprites();
        }
    }

    [MenuItem("Shifters/Clear floors")]
    private static void clearFloors() {
        foreach (Room room in GameObject.FindObjectsOfType<Room>()) {
            room.clearFloorSprites();
        }
    }

    [MenuItem("Shifters/Snap all to grid")]
    private static void snapAllToGrid() {
        foreach (TileItem item in GameObject.FindObjectsOfType<TileItem>()) {
            item.SnapToGrid();
        }
    }
}
