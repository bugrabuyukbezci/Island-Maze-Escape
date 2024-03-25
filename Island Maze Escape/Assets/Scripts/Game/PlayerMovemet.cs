using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemet : MonoBehaviour
{
    public float speed = 5;
    public float rotationSpeed = 270;
    public Animator animator;
    private CharacterController characterController;    
    public float jumpSpeed = 2f;
    float verticalSpeed;
    bool isJumping;
    public TextMesh playerNameTxt;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>(); 
    }    
    void Update()
    {
        if (!GameManager.instance.gameStart)
            return; 

        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalIput = Input.GetAxis("Vertical"); 
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalIput); 
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;         
        movementDirection.Normalize(); // 0 veya 1 olarak ver.       
        Vector3 velocity = movementDirection * magnitude; 

        if (characterController.isGrounded) 
        {
            verticalSpeed = 0;
            if(Input.GetButtonDown("Jump") && !isJumping)
            {
                isJumping = true;
                SoundManager.instance.playClip("jump");
                verticalSpeed = jumpSpeed;
                animator.Play("Jump", -1, 0);
                StartCoroutine(wait());
                IEnumerator wait()
                {
                    yield return new WaitForSeconds(1.1f);
                    isJumping = false;
                    animator.Play("Idle", -1, 0);
                }
            }
        }
        // yerçekimi uygulamasý
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime); 
        if(movementDirection != Vector3.zero) 
        {
            animator.SetBool("IsMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            // a noktasýnda b noktasýna belirlenen hýzda yumuþak dönüþ.
        }
        else // haraket yoksa idle animasyonu çalýþacak
        {
            animator.SetBool("IsMoving", false);
        }
    }
   
}
