using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LastImageController : ImageController {

    public string sceneName;

    protected override void NextImage()
    {

        SceneManager.LoadScene(sceneName);
    }

}
