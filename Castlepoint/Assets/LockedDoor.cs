using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LockedDoor : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject abovePlayer;
    [SerializeField] private GameObject thisDoor;
    [SerializeField] private GameObject pressurePad;
    private bool isLocked;
    private void OnEnable()
    {
        animator.SetBool("isLocked", true);
        isLocked = true;
        abovePlayer = GameObject.Find("abovePlayer");
    }
    private void FixedUpdate() {
        if(pressurePad != null && (int) pressurePad.GetComponent<pressure_pad>().GetPressurePadState() == 1)
        {
            StartCoroutine(UnlockDoor());
        }
    }
    private void OnBecameVisible()
    {
        if (isLocked)
        { abovePlayer.SetActive(false); }
    }
    private void OnBecameInvisible()
    {
        abovePlayer.SetActive(true);
    }
    private IEnumerator UnlockDoor()
    {
        animator.SetBool("isLocked", false);
        isLocked = false;
        yield return new WaitForSeconds(.8f);
        Debug.Log("yup");
        abovePlayer.SetActive(true);
        this.gameObject.SetActive(false);     
    }
    private void OnCollisionEnter2D(Collision2D other)
    {        
        if (other.gameObject.GetComponent<PlayerController>().hasKey == true && isLocked && pressurePad == null)
        {            
            StartCoroutine(UnlockDoor());
        }        
    }


}
