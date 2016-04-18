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
        INSTANCE = this;
    }

    void Start() {
        foreach (FurnitureType type in startingTypes) {
            getData(type).nScansCompleted += nScansPerUnlock;
        }

        UnlockStateChanged();
    }

    public void completeScanOn(FurnitureType type) {
        getData(type).nScansCompleted++;
        UnlockStateChanged();
    }

    public void reset(FurnitureType type) {
        getData(type).nScansCompleted = 0;
        UnlockStateChanged();
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
