using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoopTimer : MonoBehaviour
{
    public string firstSceneName; // Set this in the Inspector to the name of your first scene.
    public float delayInSeconds = 5.0f; // Set the time delay in seconds.

    private void Start()
    {
        StartCoroutine(TransitionAfterDelay());
    }

    private IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(firstSceneName);
    }
}
