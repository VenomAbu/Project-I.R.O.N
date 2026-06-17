using UnityEngine;

public class BounceProjectile : MonoBehaviour
{
    public float speed = 7f;
    public int maxBounces = 6;
    private int currentBounces = 0;
    private int damage;

    private Vector2 direction;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Começa com uma direção aleatória
        direction = Random.insideUnitCircle.normalized;

        // Destrói o objeto depois de 15 f
        Destroy(gameObject, 15f);
    }

    void Update()
    {
        // Adiciona movimento ao projétil
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Aponta o sprite na direção certa.
        RotateSpriteToDirection();

        // Checa se está fora da câmera
        CheckCameraBounds();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Tenta pegar o componente de vida do inimigo
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void Setup(int damageValue)
    {
        damage = damageValue;
    }

    void CheckCameraBounds()
    {
        // Converte a posição do mundo para o espaço da câmera (0 a 1)
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        bool bounced = false;

        // Checa se bateu no eixo X
        if (viewportPos.x <= 0 || viewportPos.x >= 1)
        {
            direction.x *= -1; // Inverte horizontal
            bounced = true;
        }

        // Checa se bateu no eixo Y
        if (viewportPos.y <= 0 || viewportPos.y >= 1)
        {
            direction.y *= -1; // Inverte vertical
            bounced = true;
        }

        if (bounced)
        {
            AplicarVariacaoAleatoria();
            currentBounces++;

            // Reposiciona levemente para não "prender" na borda
            ClampPosition(ref viewportPos);

            if (currentBounces >= maxBounces) Destroy(gameObject);
        }
    }

    void AplicarVariacaoAleatoria()
    {
        // Varia um pouco o ângulo ao ricochetear
        float offset = Random.Range(-10f, 10f);
        direction = Quaternion.Euler(0, 0, offset) * direction;
    }

    void ClampPosition(ref Vector3 vPos)
    {
        // Garante que o objeto volte para dentro da área da câmera (0-1)
        vPos.x = Mathf.Clamp(vPos.x, 0.01f, 0.99f);
        vPos.y = Mathf.Clamp(vPos.y, 0.01f, 0.99f);
        transform.position = cam.ViewportToWorldPoint(vPos);
    }

    void RotateSpriteToDirection()
    {
        // Calcula o ângulo baseado no vetor de direção, -90f corrige o fato do sprite estar apontado para cima
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Aplica a rotação no eixo Z (que é o eixo de rotação em 2D)
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}