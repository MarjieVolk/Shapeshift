using System;
using System.Collections.Generic;
using UnityEngine;

class UnlockState : MonoBehaviour {

    public static UnlockState INSTANCE;

    public delegate void UnlockStateChangeHandler();
    public event UnlockStateChangeHandler UnlockStateChanged;

    public int nScansPerUnlock = 3;
    public FurnitureType[] startingTypes;

    private Dictionary<FurnitureType, UnlockStateData> state = new Dictionary<FurnitureType, UnlockStateData>();

    void Awake() {
        if (INSTANCE != null) {
            Debug.Log("Unlock state already present, destroying self");
            Destroy(this);
        } else {
            Debug.Log("Becoming the MASTER UNLOCK STATE");
            INSTANCE = this;
        }

        foreach (FurnitureType type in startingTypes) {
            Debug.Log("Adding " + type);
            getData(type).nScansCompleted += nScansPerUnlock;
        }

        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }
    }

    public void completeScanOn(FurnitureType type) {
        getData(type).nScansCompleted++;
        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }
    }

    public void reset(FurnitureType type) {
        getData(type).nScansCompleted = 0;
        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }
    }

    // Returns the quality level unlocked by the player for this FurnitureType.  
    // 0 is the base quality level.  If not unlocked at all, returns -1;
    public int getQualityLevel(FurnitureType type) {
        return (getData(type).nScansCompleted / nScansPerUnlock) - 1;
    }

    public bool isUnlocked(FurnitureType type) {
        return getQualityLevel(type) != -1;
    }

    public List<FurnitureType> getUnlocked() {
        List<FurnitureType> unlockedTypes = new List<FurnitureType>();
        foreach (FurnitureType type in state.Keys) {
            if (isUnlocked(type)) {
                unlockedTypes.Add(type);
            }
        }

        return unlockedTypes;
    }

    private UnlockStateData getData(FurnitureType type) {
        if (state.ContainsKey(type)) {
            return state[type];
        } else {
            UnlockStateData data = new UnlockStateData();
            state[type] = data;
            return data;
        }
    }

    private class UnlockStateData {
        public int nScansCompleted = 0;
    }
}
