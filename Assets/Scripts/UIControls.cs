using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour {

    public GameObject pauseMenu;

    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseMenu.activeSelf) {
                pauseMenu.SetActive(false);
            }
            else if (!pauseMenu.activeSelf) {
                pauseMenu.SetActive(true);
            }
        }
	}
}
