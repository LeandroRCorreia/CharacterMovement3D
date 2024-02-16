using UnityEngine;

public static class GameTags
{
    public readonly static string Obstacle = "Obstacle";
}

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeedXZ = 10;
    [SerializeField] private float acceleration = 80;
    [SerializeField] private float deceleration = 100;
    [SerializeField] private Vector3 velocity;
    private Vector3 targetVelocity;
    private Vector3 direction;

    [Header("Collision")]
    [SerializeField] private Vector3 colliderSize;
    private Vector3 ColliderExtents => colliderSize * 0.5f;
    private Vector3 ColliderCenter => transform.position + ColliderExtents.y * Vector3.up;
    private RaycastHit[] hits = new RaycastHit[15];

    public void SetInput(in Vector3 input)
    {

        direction = input.normalized;
        targetVelocity = direction * maxSpeedXZ;
    }

    void FixedUpdate()
    {
        Vector3 lastPosition = transform.position;

        velocity = UpdateVelocity();
        CheckHorizontalCollision();

        Vector3 targetPosition = lastPosition + velocity * Time.fixedDeltaTime;

        transform.position = Vector3.MoveTowards(lastPosition, targetPosition, Time.fixedDeltaTime * maxSpeedXZ);
    }

    private Vector3 UpdateVelocity()
    {
        Vector3 copyCurrentVelocity;

        if(velocity.sqrMagnitude <= targetVelocity.sqrMagnitude)
        {
            copyCurrentVelocity = Vector3.MoveTowards(velocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        }
        else
        {
            copyCurrentVelocity = Vector3.MoveTowards(velocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        return copyCurrentVelocity;
    }

    private void CheckHorizontalCollision()
    {
        var rayLenght = velocity * Time.fixedDeltaTime;
        int hitCount = Physics.BoxCastNonAlloc(ColliderCenter, ColliderExtents, direction, hits, Quaternion.identity, rayLenght.magnitude);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = hits[i];
            if (hit.transform.gameObject.CompareTag(GameTags.Obstacle))
            {
                Vector3 projectedVector = CalculateWallSliding(hit);
                velocity = new Vector3(projectedVector.x, velocity.y, projectedVector.z);
            }

        }

    }

    private Vector3 CalculateWallSliding(RaycastHit hit)
    {
        Vector3 projectedVector;

        var inverseNormal = hit.normal * -1;
        var velocityMagnitude = velocity.magnitude;
        var dot = Vector3.Dot(velocity, inverseNormal);
        var cos = dot / (velocityMagnitude / inverseNormal.magnitude);

        var isApproximatellyCollinears = Mathf.Approximately(cos, 1.0f) || cos >= 0.98f && cos <= 1;
        if (!isApproximatellyCollinears)
        {
            projectedVector = Vector3.ProjectOnPlane(velocity, hit.normal);
        }
        else
        {
            projectedVector = Vector3.ProjectOnPlane(Quaternion.Euler(0, 45, 0) * velocity, hit.normal);
        }

        return projectedVector.normalized * velocityMagnitude;
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + ColliderExtents.y * Vector3.up, colliderSize);


        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, velocity);


    }

}
