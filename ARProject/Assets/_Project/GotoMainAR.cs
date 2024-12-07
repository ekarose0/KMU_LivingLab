using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMainAR : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Logo;
    public Animator Bird;
    public Animator Uidole;

    void Start()
    {
        StartCoroutine(StartMain());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAnimation(string name)
    {
        Uidole.SetBool(name, true);
    }

    IEnumerator StartMain()
    {
        yield return new WaitForSeconds(5.0f);

        Logo.SetActive(false);
    }

    public void StartGoByName(string sceneName)
    {
        StartCoroutine(StartGoScene(sceneName));
    }

    IEnumerator StartGoScene(string sceneName)
    {
        Bird.SetBool("GO", true);
        Uidole.SetBool("GO", true);

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
