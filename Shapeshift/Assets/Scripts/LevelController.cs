using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    public AudioClip levelAdvanceSound;

    public static LevelController INSTANCE;

    public int nLevels;

    private int currentLevel;

	// Use this for initialization
	void Awake () {
        //if (INSTANCE == null) {
            INSTANCE = this;
            DontDestroyOnLoad(this);

            if (SceneManager.GetActiveScene().buildIndex == 0) {
                // TODO: main menu first
                SceneManager.LoadScene(1);
            }

            gameObject.AddComponent<AudioSource>();
        //} else {
        //    Destroy(this);
        //}
    }

    void OnLevelWasLoaded(int level) {
        currentLevel = level;

        VictoryTotem.INSTANCE.OnVictory += () => {
            advanceToNextLevel();
        };
    }

	public void advanceToNextLevel() {
        // TODO: level/card select first

        GetComponent<AudioSource>().PlayOneShot(levelAdvanceSound);

        if (currentLevel + 1 <= nLevels) {
            SceneManager.LoadScene(currentLevel + 1);
        } else {
            // TODO: win screen
        }
    }
}
