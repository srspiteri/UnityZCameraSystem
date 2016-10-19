using UnityEngine;
using System.Collections;

public class ZCameraControl : MonoBehaviour
{
    /*
     *      The Z Camera is a camera system which follows the player.
     *      
     *      The camera is located behind the player with an elevation above the floor, 
     *      In addition the camera will always looks at the player. 
     *      
     *      The camera's position is updated over time, so it has to catch up to the player.
     */

    private float zCameraDistance   = 5f;
    private float zCameraDistanceUp = 5f;    
    private float zCameraSmoothness = 0.1f;   //The smoothness of the camera's transitions

    private Transform   zCameraFollow;        //The object the camera is following
    private Vector3     zCameraTargetPosition;//The desired position of the camera;

    private Vector3 zCameraLookDirection;

    private Vector3 velocity = Vector3.zero;

    private Vector3 zCameraOffset = new Vector3(0f, 1.5f, 0f);

    #region Methods

    //Finds the player character within the current scene
    private void zCameraFindPlayer() 
    {
        zCameraFollow = GameObject.FindWithTag("Player").transform;
    }

    //Updates the position of the camera relative to the player
    private void zCameraUpdate()
    {
        if (zCameraFollow)
        {
            //The camera's position near the  character is offset
            Vector3 characterOffset = zCameraFollow.position + zCameraOffset;
            zCameraLookDirection = characterOffset - this.transform.position;

            //Get the camera's direction
            zCameraLookDirection.y = 0;
            zCameraLookDirection.Normalize();

            /*
             * The desired position of the camera is calculated
             * This uses the position of the player, 
             * The camera is also elevated off the floor by an offset using 'zCameraDistanceUp'
             * The camera is placed [???] with a distance offset using 'zCameraDistance'
             */
            zCameraTargetPosition = (characterOffset + zCameraFollow.up * zCameraDistanceUp ) - (zCameraLookDirection * zCameraDistance);


            /*
             * The camera's position is updated gradually to the intended position in space
             */
            zCameraSmoothPosition(this.transform.position, zCameraTargetPosition);

            transform.LookAt(zCameraFollow);
        }
    }

    void zCameraSmoothPosition(Vector3 fromPos, Vector3 toPos)
    {
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocity, zCameraSmoothness);
    }

    //Draw/Print Debug Information related to the camera system
    private void zCameraDebug()
    {
        //Height of camera away from floor
        Debug.DrawRay(zCameraFollow.position, Vector3.up * zCameraDistanceUp, Color.red);

        //Distance of camera behind player
        Debug.DrawRay(zCameraFollow.position, -1f * zCameraFollow.forward * zCameraDistance, Color.blue);

        //The bee-line which the camera will have to travel to between it's old and new position
        Debug.DrawLine(zCameraFollow.position, zCameraTargetPosition, Color.magenta);
    }

    
    void Start()
    {
        zCameraFindPlayer();
    }

    void LateUpdate()
    {
        zCameraUpdate();

        //zCameraDebug();
    }

    #endregion
}