using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(AudioSource))]
public class LevelController : MonoBehaviour {

    public AudioClip levelAdvanceSound;

    public static LevelController INSTANCE;

    public int nLevels;

    private int currentLevel;

	// Use this for initialization
	void Awake () {
        if (INSTANCE == null) {
            INSTANCE = this;
            DontDestroyOnLoad(this);

            currentLevel = SceneManager.GetActiveScene().buildIndex;
        } else {
            Destroy(this);
        }
    }

    void Start() {
        VictoryTotem.INSTANCE.OnVictory += () => {
            advanceToNextLevel();
        };
    }

    void OnLevelWasLoaded(int level) {
        currentLevel = level;
    }

	public void advanceToNextLevel() {
        // TODO: level/card select first

        GetComponent<AudioSource>().PlayOneShot(levelAdvanceSound);

        if (currentLevel + 1 <= nLevels) {
            SceneManager.LoadScene(currentLevel + 1);
        }
    }
}
