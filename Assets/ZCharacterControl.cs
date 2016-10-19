using UnityEngine;
using System.Collections;

public class ZCharacterControl : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Animator zCharAnimator;
    [SerializeField]
    private float directionDampTime = .25f;

    private float zCharSpeed = 0.0f;
    private float zCharDirection = 0f;
    private float zCharH = 0.0f;
    private float zCharV = 0.0f;
    private float zCharTurnSpeed = 5f;

    [SerializeField]
    private ZCameraControl zCamera;


    #endregion

    //Method used for updating the state of the animator

    private void zCharAnimation()
    {
        //Only works if animator exists
        if (zCharAnimator)
        {
            //Get Axis Inputs
            zCharH = Input.GetAxis("Horizontal");
            zCharV = Input.GetAxis("Vertical");

            /*
             * This method calculates both the characters movement direction and speed
             * Using references, it makes it possible to calculate to values at once instead of returning a single value             * 
             */
            zCharInputToWorldSpace(this.transform, zCamera.transform, ref zCharDirection, ref zCharSpeed);

            //Set the animator values based on this new information
            zCharAnimator.SetFloat("Speed", zCharSpeed);
            zCharAnimator.SetFloat("Direction", zCharDirection, directionDampTime, Time.deltaTime);


        }
    }

    void zCharInputToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut)
    {
        //Get the character's forward facing direction
        Vector3 rootDirection = root.forward;

        //Get the input direction and magnitude
        Vector3 stickDirection = new Vector3(zCharH, 0, zCharV);
        speedOut = stickDirection.sqrMagnitude;

        //Get the forward facing direction of the camera
        Vector3 cameraDirection = camera.forward;

        //Kill the Y Axis of the camera's direction since the stick direction doesnt have a y rotation
        cameraDirection.y = 0.0f;

        /*
         * The rotation difference is calculated between the camera direction and the forward facing direction
         * This is done to get a rotation 'shift' to be applied to the input direction
         */
        Quaternion shift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

        /*
         * The input direction is offset by this shift rotation using quaternion multiplication, effectively 'adding' the angles together
         * This results in an inpt direction relative to the camera for camera relative movement
         * 
         * When the stick is pushed forwards, the direction is away from the camera, 
         * where as the stick pushed backwards is towards it.
         */
        Vector3 moveDirection = shift * stickDirection;

        /*
         * Find the direction in between the two directions 
         * (i.e a vector perpendicular to the other two using left hand rule 
         * 
         * Direction A = thumb, 
         * Direction B = Index Finger
         * 
         * This is used to heelp determine the 'direction of rotation' whilst changing direction of movement
         */
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        /*
         * Get the angle between the characters facing direction and desired movement direction
         * Using the axis sign, determine the direction of rotation
         * Multiply this result (-1 or 1) by the angle
         * 
         * This results in the size of the rotation the character will turn and in which direction
         * 
         * Dividing this value by 180 (i.e a hemisphere), the result is a value between -1 and 1!
         */
        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        angleRootToMove /= 180f;

        //Finally multiply the final direction of rotation (between -1 and 1) with the characters turn speed.
        directionOut = angleRootToMove * zCharTurnSpeed;

    }


    // Use this for initialization
    void Start()
    {
        //Setup Animator
        zCharAnimator = GetComponent<Animator>();
        if (zCharAnimator.layerCount >= 2)
            zCharAnimator.SetLayerWeight(1, 1);
    }

    void FixedUpdate()
    {
        //DO STUFF HERE NEXT
    }

    // Update is called once per frame
    void Update()
    {
        zCharAnimation();
	}
}
