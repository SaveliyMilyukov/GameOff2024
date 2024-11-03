using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    public AudioSource source;

    public void LoadScene(string sceneName_)
    {
        SceneManager.LoadScene(sceneName_);
    }

    public void PlaySound(AudioClip sound_)
    {
        source.PlayOneShot(sound_);
    }
}
