using UnityEngine;

public class TankMovement : MonoBehaviour
{
    // Ace Combat Mode
    [SerializeField] Rigidbody2D rb;
   public float accelerationSpeed = 1f;
   public float maxSpeed;
   public float terrainResistance;
   public float rotationSpeed;
   public float maxReverseSpeed;
   public float currentThrottle;

    // Inputs Variables
    float inputVertical;
    float inputHorizontal;

    private void Update()
    {
        // Input Data Gathering
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
    }

    void MoveTank()
    {
        // Accelerate
        currentThrottle += inputVertical * accelerationSpeed * Time.fixedDeltaTime;

        // Lose speed over time

       
            if (currentThrottle > 0)
                currentThrottle -= terrainResistance * Time.fixedDeltaTime;
            else if (currentThrottle < 0)
                currentThrottle += terrainResistance * Time.fixedDeltaTime;

        // Cap velocity between maxSpeed and maxReverseSpeed
        currentThrottle = Mathf.Clamp(currentThrottle, maxReverseSpeed, maxSpeed);

        // Rotation
        float rotation = inputHorizontal * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation - rotation);

        // Velocity application
        rb.linearVelocity = transform.up * currentThrottle;

        Debug.Log(currentThrottle);
    }
}
