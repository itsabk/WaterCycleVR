using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using EnviroSamples;

public class ButtonBehaviour : MonoBehaviour
{


    public GameObject disableGameObject;
    public GameObject enableGameObject;
    public int weatherID;
    public string weatherName;
    public string animationName;
    public Animator animator;



    public void WhenClicked()
    {

        disableGameObject.SetActive(false);
        //EnviroSky.instance.ChangeWeather(weatherID);
        //EnviroSky.instance.ChangeWeather(weatherName);
        enableGameObject.SetActive(true);
        animator.SetTrigger(animationName);

    }

}
