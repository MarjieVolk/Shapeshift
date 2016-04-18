﻿using System;
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
            Destroy(this);
        } else {
            INSTANCE = this;
            DontDestroyOnLoad(this);
        }

        foreach (FurnitureType type in startingTypes) {
            getData(type).lockedInScansCompleted += nScansPerUnlock;
        }

        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }
    }

    void OnLevelWasLoaded(int level) {
        foreach(FurnitureType type in state.Keys) {
            UnlockStateData data = getData(type);
            data.lockedInScansCompleted += data.thisLevelScansCompleted;
            data.thisLevelScansCompleted = 0;
            data.temporarilyLocked = false;
        }

        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }

        PlayerCaughtHandler caughtHandler = FindObjectOfType<PlayerCaughtHandler>();

        if (caughtHandler != null) {
            caughtHandler.PlayerCaughtAsFurniture += (FurnitureType type) => {
                Debug.Log("Locking " + type);
                getData(type).temporarilyLocked = true;

                if (UnlockStateChanged != null) {
                    UnlockStateChanged();
                }
            };

            caughtHandler.PlayerCaughtAsHuman += () => {
                foreach (FurnitureType type in state.Keys) {
                    UnlockStateData data = getData(type);
                    data.temporarilyLocked = false;
                    data.thisLevelScansCompleted = 0;
                }

                if (UnlockStateChanged != null) {
                    UnlockStateChanged();
                }
            };
        }
    }

    public void completeScanOn(FurnitureType type) {
        getData(type).thisLevelScansCompleted++;
        if (UnlockStateChanged != null) {
            UnlockStateChanged();
        }
    }

    // Returns the quality level unlocked by the player for this FurnitureType.  
    // 0 is the base quality level.  If not unlocked at all, returns -1;
    public int getQualityLevel(FurnitureType type) {
        return ((getData(type).thisLevelScansCompleted + getData(type).lockedInScansCompleted) / nScansPerUnlock) - 1;
    }

    public bool isTemporarilyLocked(FurnitureType type) {
        return getData(type).temporarilyLocked;
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
        public int lockedInScansCompleted = 0;
        public int thisLevelScansCompleted = 0;
        public bool temporarilyLocked = false;
    }
}
