using UnityEngine;

public class MainTank : Character
{
    [SerializeField] Rigidbody2D rb;
    public float accelerationSpeed = 1f;
    public float maxSpeed;
    public float terrainResistance;
    public float rotationSpeed;
    public float maxReverseSpeed;
    public float gas;
    public float coins;

    private float gasTime = 0;
    public int ammo;

    private float currentThrottle;

    float inputVertical;
    float inputHorizontal;

    private Inventory inventory;
    public UI_Inventory uiInventory;

    protected override void Awake()
    {
        // Máximiza a vida atual
        base.Awake();

        // Inicia o inventário do tanque
        inventory = new Inventory();
    }

    private void Start()
    {
        uiInventory.SetInventory(inventory, this);
    }

    private void Update()
    {
        // Pega os Inputs do jogador
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");

        // Mantem o crônometro que gasta gasolina ativo
        UseGas();
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

        // Deixa o limite de velocidade entre maxSpeed e maxReverseSpeed
        float limiteMaximo = maxSpeed;
        float limiteMinimo = maxReverseSpeed;

        // Se a gasolina acabou, corta os limites pela metade
        if (gas <= 0)
        {
            limiteMaximo = maxSpeed / 2f;
            limiteMinimo = maxReverseSpeed / 2f;
        }

        // Deixa a velocidade travada entre os limites calculados
        // Codar reset de velocidade quando o tanque para de andar no futuro pode ser uma boa ideia.
        currentThrottle = Mathf.Clamp(currentThrottle, limiteMinimo, limiteMaximo);

        // Aplica a velocidade
        rb.linearVelocity = transform.up * currentThrottle;

        // Rotaciona o objeto baseado na velocidade de rotação
        float rotation = inputHorizontal * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation - rotation);
    }

    public void UseGas()
    {
        if (Mathf.Abs(currentThrottle) > 0.05f)
        {
            // Inicia o cronômetro que mede quando gastar a gasolina.
            gasTime += Time.deltaTime;

            // Quando o cronômetro bater 1 segundo (ou mais)...
            if (gasTime >= 1f)
            {
                gas -= 1f;      
                gasTime -= 1f;   // Reseta o cronômetro

                // Trava de segurança para a gasolina não ficar negativa
                if (gas < 0)
                {
                    gas = 0;
                    // Aqui você pode adicionar lógica pro tanque parar de andar no futuro!
                }
            }
        }
    }

    public override void Die()
    {
        Debug.Log("O tanque foi destruído! Fim de jogo.");

        // Procura o Manager na cena e ativa a tela de morte
        GameOverManager manager = FindFirstObjectByType<GameOverManager>();
        if (manager != null)
        {
            manager.ShowGameOver();
        }

        // Faz o tanque desaparecer
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemWorld itemWorld = collision.gameObject.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            Item collectedItem = itemWorld.GetItem();

            // --- FILTRO DE COLETA ---
            if (collectedItem.itemType == Item.ItemType.Coin)
            {
                // Se for moeda, soma o valor na carteira e avisa no console
                coins += collectedItem.amount;
                Debug.Log($"+$! Pegou uma moeda. Total na carteira: {coins}");
            }
            else
            {
                // Se for qualquer outro item, vai para o inventário normalmente
                inventory.AddItem(collectedItem);
            }

            // O objeto físico da cena é destruído
            itemWorld.DestroySelf();
        }
    }

}