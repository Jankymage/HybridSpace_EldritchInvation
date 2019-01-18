using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public void StartGame() {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void ExitGame() {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void CloseGame() {
        Application.Quit();
    }

}
