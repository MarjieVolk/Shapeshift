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
            SceneManager.LoadScene(1);

        } else {
            Destroy(this);
        }
	}

    void OnLevelWasLoaded(int level) {
        currentLevel = level;

        VictoryTotem.INSTANCE.OnVictory += () => {
            advanceToNextLevel();
        };
    }

	public void advanceToNextLevel() {
        // TODO: level/card select first

        if (currentLevel + 1 <= nLevels) {
            SceneManager.LoadScene(currentLevel + 1);
        } else {
            // TODO: win screen
        }
    }
}
