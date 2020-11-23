using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animaBotaoConfig : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void animaConfig()
    {
       
            animator.SetTrigger("configTrigger");

   
    }

    public void hideVolume()
    {

        animator.SetTrigger("");


    }
}
