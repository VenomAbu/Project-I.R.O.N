using UnityEngine;

public class MainTank : Character
{
    [SerializeField] Rigidbody2D rb;
    public float accelerationSpeed = 1f;
    public float maxSpeed;
    public float terrainResistance;
    public float rotationSpeed;
    public float maxReverseSpeed;
    private float currentThrottle;

    float inputVertical;
    float inputHorizontal;

    private void Update()
    {
        // Pega os Inputs do jogador
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        MoveTank();
    }

    void MoveTank()
    {
        // Calculo da potência atual
        currentThrottle += inputVertical * accelerationSpeed * Time.fixedDeltaTime;

        // Perde velocidade com o tempo caso haja Resistência
        if (currentThrottle > 0)
            currentThrottle -= terrainResistance * Time.fixedDeltaTime;
        else if (currentThrottle < 0)
            currentThrottle += terrainResistance * Time.fixedDeltaTime;

        // Deixa a velocidade entre maxSpeed e maxReverseSpeed
        currentThrottle = Mathf.Clamp(currentThrottle, maxReverseSpeed, maxSpeed);

        // Aplica a velocidade
        rb.linearVelocity = transform.up * currentThrottle;

        // Rotaciona o objeto baseado na velocidade de rotação
        float rotation = inputHorizontal * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation - rotation);
    }
    public override void Die()
    {
        Debug.Log("O tanque foi destruído! Fim de jogo.");
        
        // Faz o tanque desaparecer
        gameObject.SetActive(false);
    }

}