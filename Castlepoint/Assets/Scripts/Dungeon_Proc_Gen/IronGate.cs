using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronGate : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject pressurePad;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("open", false);
        anim.SetBool("closed", true);
    }
    private void FixedUpdate() 
    {
        if(pressurePad != null && (int) pressurePad.GetComponent<pressure_pad>().GetPressurePadState() == 1)
        {
            OpenGate();
        }
        else{CloseGate();}
    }
    private void OpenGate()
    {
        anim.SetBool("closed", false);
        anim.SetBool("open", true);
    }
    private void CloseGate()
    {
        anim.SetBool("open", false);
        anim.SetBool("closed", true);
    }
}
