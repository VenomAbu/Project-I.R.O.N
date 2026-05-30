using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

// Esta estrutura guarda as informações de uma única "tela" de fala
[System.Serializable]
public struct DialogueLine
{
    public string characterName;
    public Sprite portrait;

    // TextArea faz a caixinha de texto no Inspector ficar maior
    [TextArea(3, 10)]
    public string sentence;

    [Header("Qual Câmera? (Deixe vazio para manter a atual)")]
    public Unity.Cinemachine.CinemachineCamera cameraDaFala;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Componentes da UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Câmera Padrão do Jogo")]
    public Unity.Cinemachine.CinemachineCamera playerCam;

    // Variável invisível no Inspector para o código lembrar quem está no ar
    private Unity.Cinemachine.CinemachineCamera cameraAtualDaCutscene;

    [Header("Atores da Cutscene")]
    public GameObject hordaFalsa;

    [Header("O Roteiro da Cena")]
    public DialogueLine[] lines; // Uma lista com todas as falas da cutscene
    private int currentLineIndex = 0;

    private void Start()
    {
        // Ao iniciar, mostra a primeira fala
        StartDialogue();
    }

    private void Update()
    {
        // Se o jogador apertar Espaço ou clicar com o mouse, avança o diálogo
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLineIndex = 0;

        // Pausa os inimigos e o jogador
        Time.timeScale = 0f;

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Checa se o roteiro acabou
        if (currentLineIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        // Pega a linha atual do roteiro
        DialogueLine currentLine = lines[currentLineIndex];

        // ==========================================
        // --- NOVO: SISTEMA DE CÂMERA MODULAR ---
        // ==========================================

        // Checa se você escolheu uma câmera específica para esta linha no Inspector
        if (currentLine.cameraDaFala != null)
        {
            // Se já existia uma câmera de cutscene ligada na tela, desliga ela
            if (cameraAtualDaCutscene != null)
            {
                cameraAtualDaCutscene.gameObject.SetActive(false);
            }
            // Se for a primeira vez mudando a câmera, desliga a câmera do Tanque
            else
            {
                playerCam.gameObject.SetActive(false);
            }

            // Liga a câmera nova e salva ela na memória como a "Câmera Atual"
            currentLine.cameraDaFala.gameObject.SetActive(true);
            cameraAtualDaCutscene = currentLine.cameraDaFala;
        }

        // ==========================================

        // 1. Atualiza o texto principal da fala/narrativa
        dialogueText.SetText(currentLine.sentence);

        // 2. Sistema do Nome (Checa se está vazio)
        if (string.IsNullOrEmpty(currentLine.characterName))
        {
            // Se estiver vazio, desliga o objeto do nome
            nameText.gameObject.SetActive(false);
        }
        else
        {
            // Se tiver nome, liga o objeto e escreve
            nameText.gameObject.SetActive(true);
            nameText.SetText(currentLine.characterName);
        }

        // 3. Sistema do Retrato (Checa se tem foto)
        if (currentLine.portrait == null)
        {
            // Se não tiver foto (modo narrador), desliga o quadro de imagem
            portraitImage.gameObject.SetActive(false);
        }
        else
        {
            // Se tiver foto, liga o quadro e coloca o sprite
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = currentLine.portrait;
        }

        // Prepara para a próxima vez que o jogador clicar
        currentLineIndex++;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (hordaFalsa != null) hordaFalsa.SetActive(false); // Demite os figurantes

        // --- DEVOLVE A CÂMERA PARA O TANQUE ---
        if (cameraAtualDaCutscene != null)
        {
            cameraAtualDaCutscene.gameObject.SetActive(false);
        }
        playerCam.gameObject.SetActive(true);

        // Limpa a memória para a próxima cutscene
        cameraAtualDaCutscene = null;

        StartCoroutine(ResumeTimeNextFrame());
    }

    private IEnumerator ResumeTimeNextFrame()
    {
        // manda o código pausar aqui e esperar o frame atual acabar
        yield return null;

        // No frame seguinte volta o tempo com segurança!
        Time.timeScale = 1f;
    }
}