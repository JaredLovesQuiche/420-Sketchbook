using UnityEngine;

public class WalkerAI : MonoBehaviour
{
    public int Whiskers;

    public float RaycastDistance;
    public float MovementDirectionBias; // Value between -1.0f and 1.0f.

    public float RaycastDistanceOffset = 0.3f;

    public Vector3 StoredMoveDirection = Vector3.forward;

    public Ray[] Rays;

    public Vector3 RayPath(Transform Target)
    {
        Vector3 NewMoveDirection = Vector3.zero;

        // Create array containing all direct directions to target.

        Rays = new Ray[Whiskers];

        for (int i = 0; i < Whiskers; i++)
        {
            float Rotation = Mathf.PI / (Whiskers / 2) * i; // Rotation increment based on # of whiskers

            Rays[i].Weight = 0;

            Rays[i].Direction = new Vector3(Mathf.Cos(Rotation), 0, Mathf.Sin(Rotation));

            Vector3 RaycastStartPos = transform.position + Rays[i].Direction * RaycastDistanceOffset;
            Physics.Raycast(RaycastStartPos, Rays[i].Direction, out Rays[i].Raycast, 2.0f);

            // If ray hits something, reduce the weight of that direction to avoid crashing into other things.
            if (Rays[i].Raycast.collider)
            {
                Rays[i].Weight = -2.0f;
            }

            // Add the dot product of ray direction and direction to target
            Vector3 directionToTarget = FlattenVector3(Target.position - transform.position).normalized;
            float dot = Vector3.Dot(directionToTarget, Rays[i].Direction);
            Rays[i].Weight += dot;

            // Finally, Add aux bonuses to weight

            // Make final movement direction biased towards the direction it is already moving in
            Rays[i].Weight += Map(Vector3.Dot(Rays[i].Direction, StoredMoveDirection), -1, 1, 0, MovementDirectionBias);

            #region Debug Lines

            Debug.DrawLine(RaycastStartPos, transform.position + Rays[i].Direction * Map(Rays[i].Weight, -1, 1, 0, 1) * 3,
                new Color(1 - Map(Rays[i].Weight, -1, 1, 0, 1), Map(Rays[i].Weight, -1, 1, 0, 1), 0));

            #endregion
        }

        // Average the directions of all whiskers times their weights.

        int CurrentIndex = FindBestRayIndex() - 1;
        for (int i = 0; i < 5; i++)
        {
            if (CurrentIndex < 0)
                CurrentIndex += Rays.Length;
            else if (CurrentIndex >= Rays.Length)
                CurrentIndex -= Rays.Length;
        
            NewMoveDirection += Rays[CurrentIndex].Direction * Rays[CurrentIndex].Weight;
        
            CurrentIndex++;
        }

        NewMoveDirection.Normalize();

        #region debug

        Debug.DrawLine(transform.position, transform.position + NewMoveDirection * 3, Color.blue);
        StoredMoveDirection = NewMoveDirection;

        #endregion

        return NewMoveDirection;
    }

    private int FindBestRayIndex()
    {
        int BestIndex = 0;
        float BestWeight = float.MinValue;

        for (int i = 0; i < Rays.Length; i++)
        {
            if (Rays[i].Weight > BestWeight)
            {
                BestIndex = i;
                BestWeight = Rays[i].Weight;
            }
        }

        return BestIndex;
    }

    private float Map(float Value, float InputStart, float InputEnd, float OutputStart, float OutputEnd)
    {
        float NewValue = OutputStart + ((OutputEnd - OutputStart) / (InputEnd - InputStart)) * (Value - InputStart);

        return NewValue;
    }

    private Vector3 FlattenVector3(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public struct Ray
    {
        public float Weight;
        public Vector3 Direction;
        public RaycastHit Raycast;
    }
}