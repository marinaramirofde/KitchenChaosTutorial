using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;

    private void Update(){
       HandleMovement();
       HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if( moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }


        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, 
        out RaycastHit raycastHit, interactDistance, counterLayerMask)){
            if(raycastHit.transform.TryGetComponent( out ClearCounter clearCounter)){
                // Has clearcounter
                clearCounter.Interact();
            }
        }
    }

    private void HandleInteractions(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if( moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }


        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, 
        out RaycastHit raycastHit, interactDistance, counterLayerMask)){
            if(raycastHit.transform.TryGetComponent( out ClearCounter clearCounter)){
                // Has clearcounter
            }  
        }   
    }

    private void HandleMovement(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, 
        transform.position + Vector3.up * playerHeight,
        playerRadius, 
        moveDir,
        moveDistance);

        if(!canMove){
            //Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

            bool canMoveX = !Physics.CapsuleCast(transform.position, 
            transform.position + Vector3.up * playerHeight,
            playerRadius, 
            moveDirX,
            moveDistance);

            if(canMoveX){
                moveDir = moveDirX;
            }else{
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                bool canMoveZ = !Physics.CapsuleCast(transform.position, 
            transform.position + Vector3.up * playerHeight,
            playerRadius, 
            moveDirZ,
            moveDistance);

                if(canMoveZ){
                    moveDir = moveDirZ;
                }else{
                    // Cannot move in any direction
                }
            }

        }
        if(canMove){
        transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        
    }
}
