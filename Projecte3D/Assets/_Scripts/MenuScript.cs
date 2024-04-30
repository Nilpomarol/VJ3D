using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuCanvas;
    // Start is called before the first frame update
    void Start()
    {
        gameMenuCanvas.SetActive(true);
    }

    // Update is called once per frame
    public async void StartGame()
    {
        print ("StartGame");
        gameMenuCanvas.SetActive(false);
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Additive);
    }
}
