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
        if (INSTANCE == null) {
            INSTANCE = this;
            DontDestroyOnLoad(this);

            // TODO: main menu first
            currentLevel = 1;
            SceneManager.LoadScene(currentLevel);

        } else {
            Destroy(this);
        }
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
