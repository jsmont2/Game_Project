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
        yield return new WaitForSeconds(.9f);
        thisDoor.SetActive(false);
        abovePlayer.SetActive(true);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.GetComponent<PlayerController>().hasKey == true && isLocked)
        {
            Debug.Log("yup");
            StartCoroutine(UnlockDoor());
        }
    }


}
