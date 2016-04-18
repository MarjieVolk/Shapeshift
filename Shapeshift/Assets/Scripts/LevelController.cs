using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    public static LevelController INSTANCE;

    public int nLevels;

    private int currentLevel;

	// Use this for initialization
	void Start () {
        INSTANCE = this; 
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(UnlockState.INSTANCE);

        // TODO: main menu first
        currentLevel = 1;
        SceneManager.LoadScene(currentLevel);
	}
	
	public void advanceToNextLevel() {
        // TODO: level/card select first
        currentLevel++;

        if (currentLevel <= nLevels) {
            SceneManager.LoadScene(currentLevel);
        } else {
            // TODO: win screen
        }
    }
}
