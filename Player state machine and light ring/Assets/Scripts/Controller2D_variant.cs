using System;
using UnityEngine;

public class Controller2D_variant : MonoBehaviour
{
    const float skinWidth = .068f;

    [Header("Raycast settings")]
    [Range(2, 100)] public int horizontalRayCount = 8;
    [Range(2, 100)] public int verticalRayCount = 8;
    public LayerMask collisionMask;
    [SerializeField] private LayerMask groundLayerMask;

    float maxClimbAngle = 75;
    float maxDescendAngle = 75;
    float slideSpeed = 10f;

    [SerializeField]  float gravity = Physics.gravity.y;
    Vector3 velocity;
    RaycastOrigins raycastOrigins;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    float horizontalInput;
    BoxCollider2D collider;
    public collisionInfo collisions;

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
   public struct collisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public void reset()
        {
            above = below = false;
                left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
      }

        private void Awake()
        {
        collider = GetComponent<BoxCollider2D>();
            calculateRaySpacing();
        }


        public void HorizontalCollisions(ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
                if (hit)
                {
                /* float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                  if(i==0 && slopeAngle<=maxClimbAngle)
                  {
                 if(collisions.descendingSlope)
                 {
                     collisions.descendingSlope = false;
                     velocity = collisions.velocityOld;
                 }
                 float distanceToSlopeStart = 0;
                 if(slopeAngle!=collisions.slopeAngleOld)
                 {
                     distanceToSlopeStart = hit.distance - skinWidth;
                     velocity.x -= distanceToSlopeStart * directionX;
                 }
                      climbSlope(ref velocity, slopeAngle);
                 velocity.x += distanceToSlopeStart * directionX;
                  }

                   if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                      {
                         velocity.x = Mathf.Min(Mathf.Abs(velocity.x), (hit.distance - skinWidth)) * directionX;
                 rayLength = Mathf.Min(Mathf.Abs(velocity.x) + skinWidth, hit.distance);

                 if (collisions.climbingSlope)
                         {
                     velocity.y = Mathf.Tan(collisions.slopeAngle)* Mathf.Deg2Rad*Mathf.Abs(velocity.x);
                         }

                      }*/
                collisions.left = directionX == -1;
            collisions.right = directionX == 1;
        } 
            }
        }

    /*private void climbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Math.Abs(velocity.x);
        float climbVelocityY = (float)(Math.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance);
        if(velocity.y<=climbVelocityY)
       
        {
    velocity.y = climbVelocityY;
        velocity.x = (float)(Math.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance*Mathf.Sign(velocity.x));
        collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
      
    }  
    public void descendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);

                       
                        Vector2 oppositeSideRayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                        RaycastHit2D oppositeSideHit = Physics2D.Raycast(oppositeSideRayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
                        float descendVelocityY = Mathf.Clamp(Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance, -Mathf.Infinity, oppositeSideHit.distance - skinWidth);
                        

                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;
                        collisions.slopeAngle = slopeAngle;
                        collisions.below = true;
                        collisions.descendingSlope = true;
                    }
                }
            }
           
            { 
            }
        }

    }*/

    public void VerticalCollisions(ref Vector3 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
                if (hit)
                {
                 //   velocity.y = (hit.distance - skinWidth) * directionY;
                  //  rayLength = hit.distance;

                  /*  if(collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }*/

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
          /*  if(collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Math.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }*/
        }
        private void calculateRaySpacing()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        public void updateRaycastOrigins()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }
    }