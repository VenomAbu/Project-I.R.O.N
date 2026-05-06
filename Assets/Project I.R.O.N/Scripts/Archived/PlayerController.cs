using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform visualDoTanque;

    // Speed
    [SerializeField] public float tankSpeed;

    // Arcade Mode
    [SerializeField] public float arcadeSpeed;

    // Ace Combat Mode
    [SerializeField] float accelerationSpeed = 1f;
    [SerializeField] float maxSpeed;
    [SerializeField] float terrainResistance;
    [SerializeField] public float rotationSpeed;
    [SerializeField] float maxReverseSpeed;
    float currentThrottle;

    // Inputs Variables
    float inputVertical;
    float inputHorizontal;

    // Mode Switches
    [SerializeField] public bool arcadeMode;
    [SerializeField] public bool tankMode;
    [SerializeField] public bool aceCombatTerrainResistance;

    private void Update()
    {
        // Input Data Gathering
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        // Garantindo que o protótipo não exploda
        if (arcadeMode && tankMode) return;

        // Mode Selection
        if (arcadeMode)
        {
            ArcadeMovement();
        }
        else if (tankMode)
        {
            TankMovement();
        }
        else
        {
            AceCombatMovement();
        }
    }

    void TankMovement()
    {
        // Rotation
        float rotation = inputHorizontal * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation - rotation);

        // Frontal Movement
        rb.AddForce(rb.transform.up * tankSpeed * inputVertical);

        Debug.Log(rb.linearVelocity);
    }

    // Feito com IA para testar
    void ArcadeMovement()
    {
        Vector2 inputDirection = new Vector2(inputHorizontal, inputVertical);

        if (inputDirection.magnitude >= 0.1f)
        {
            rb.linearVelocity = inputDirection.normalized * arcadeSpeed;

            float anguloAlvo = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;

            float anguloSuave = Mathf.MoveTowardsAngle(visualDoTanque.localEulerAngles.z, anguloAlvo, rotationSpeed * Time.fixedDeltaTime);
            visualDoTanque.localRotation = Quaternion.Euler(0, 0, anguloSuave);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log(rb.linearVelocity);
    }
    // Fim da IA

    void AceCombatMovement()
    {
        // Accelerate
        currentThrottle += inputVertical * accelerationSpeed * Time.fixedDeltaTime;

        // Lose speed over time

        if (aceCombatTerrainResistance)
        {
            if (currentThrottle > 0)
                currentThrottle -= terrainResistance * Time.fixedDeltaTime;
            else if (currentThrottle < 0)
                currentThrottle += terrainResistance * Time.fixedDeltaTime;
        }

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
