using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoading : MonoBehaviour
{
    [SerializeField]
    private Image  _progressBar;
    float elapsed;
    // Start is called before the first frame update
    void Start()
    {
        //start async operation 
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        //create async operation
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(7);
        gameLevel.allowSceneActivation = false;

        while (gameLevel.progress < 1)
        {
            //take the progress bar fill = async operation progress
            //set_progressBar(Mathf.Clamp01(elapsed));
            _progressBar.fillAmount = gameLevel.progress;

            if (Input.GetKeyDown(KeyCode.Space))
                     gameLevel.allowSceneActivation = true;



            //yield return new WaitForEndOfFrame();
            yield return null;

            


        }
        //when finished load the game scene
         
    }
}
