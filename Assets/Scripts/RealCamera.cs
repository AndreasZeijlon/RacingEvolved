    using UnityEngine;
    using System.Collections;
     
    public class RealCamera : MonoBehaviour{
     
        [Header("Camera Properties")]
        public float DistanceAway;                 //how far the camera is from the player.
     
        public float minDistance;                   //min camera distance
        public float maxDistance;                   //max camera distance
     
        public float DistanceUp;                    //how high the camera is above the player
        public float smooth;                        //how smooth the camera moves into place

        public Transform target;                    //the target the camera follows
        public LayerMask CamOcclusion;              //the layers that will be affected by collision
     
        public float cameraHeight;
        public float cameraPan;
        Vector3 camPosition;
        Vector3 camMask;
        Vector3 followMask;
     
        // Use this for initialization
        void Start(){
            DistanceAway = 5f;
            minDistance = 1f;
            maxDistance = 5f;
            DistanceUp = 5f; 
            smooth = 10f;
            cameraHeight = 5f;
            cameraPan = 0f;
        }
     
        void LateUpdate(){
            if(0<target.eulerAngles.x && target.eulerAngles.x<90) {
                //Driving down -> raise camera height
                DistanceUp = Mathf.Clamp(5f + (Mathf.Min(Mathf.Abs(360-target.eulerAngles.x), Mathf.Abs(0-target.eulerAngles.x))*0.3f), 5f, 7f);
            }
            else {
                //Driving up -> lower camera height
                DistanceUp = Mathf.Clamp(5f - (Mathf.Min(Mathf.Abs(360-target.eulerAngles.x), Mathf.Abs(0-target.eulerAngles.x))*0.1f), 3f, 5f);
            }
            //Offset of the targets transform (Since the pivot point is usually at the feet).
            Vector3 targetOffset = new Vector3(target.position.x, (target.position.y + 3f), target.position.z);
            Quaternion rotation = Quaternion.Euler(cameraHeight,  target.eulerAngles.y-43f, cameraPan);
            Vector3 vectorMask = Vector3.one;
            Vector3 rotateVector = rotation * vectorMask;

            //this determines where both the camera and it's mask will be.
            //the camMask is for forcing the camera to push away from walls.
            camPosition = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;

            camMask = targetOffset + Vector3.up * DistanceUp - rotateVector * DistanceAway;
     
            occludeRay(ref targetOffset);
            smoothCamMethod();
     
            transform.LookAt(target);
            DistanceAway = Mathf.Clamp(DistanceAway, minDistance, maxDistance);
     
        }
        void smoothCamMethod(){
            smooth = 10f;
            transform.position = Vector3.Lerp (transform.position, camPosition, Time.deltaTime * smooth);
        }
        void occludeRay(ref Vector3 targetFollow){
            RaycastHit wallHit = new RaycastHit();
            if(Physics.Linecast(targetFollow, camMask, out wallHit, CamOcclusion)){
                smooth = 200f;
                camPosition = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, camPosition.y, wallHit.point.z + wallHit.normal.z * 0.5f);
            }
        }
    }
     
