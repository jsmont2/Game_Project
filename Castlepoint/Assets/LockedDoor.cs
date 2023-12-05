using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LockedDoor : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject abovePlayer;
    [SerializeField] private GameObject thisDoor;
    private bool isLocked;
    private void OnEnable()
    {
        animator.SetBool("isLocked", true);
        isLocked = true;
        thisDoor = GameObject.Find("LockedDoor");
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
        Destroy(thisDoor);      
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.GetComponent<PlayerController>().hasKey == true && isLocked)
        {            
            StartCoroutine(UnlockDoor());
        }
    }


}
