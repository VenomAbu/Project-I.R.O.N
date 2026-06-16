using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;


// --- NOVA ESTRUTURA: A Escolha ---
[System.Serializable]
public struct DialogueChoice
{
    public string textDaEscolha; // O que vai estar escrito no botão
    public string proximoNodeId; // Para qual "caixa" do Twine isso vai levar

}

[System.Serializable]
public struct DialogueLine
{
    [Header("ID da Passagem (Igual ao título no Twine)")]
    public string nodeId; // Ex: "Inicio da Loja", "Comprar Arma", etc.

    public string characterName;
    public Sprite portrait;

    [TextArea(3, 10)]
    public string sentence;

    [Header("Qual Câmera?")]
    public CinemachineCamera cameraDaFala;

    [Header("Bifurcações (Deixe vazio para continuar normal)")]
    public DialogueChoice[] choices;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Configurações de Início")]
    public bool autoStart = false; // A primeira caixinha (Liga/Desliga)
    public string startNodeId = ""; // O campo complementar (Deixe vazio para o modo linear)

    [Header("Componentes da UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Sistema de Bifurcações (NOVO)")]
    public Transform choicesPanel; // O pai/molde onde os botões vão nascer
    public GameObject choiceButtonPrefab; // O botão que criaremos na Unity

    [Header("Câmera Padrão do Jogo")]
    public CinemachineCamera playerCam;
    private CinemachineCamera cameraAtualDaCutscene;

    [Header("Atores da Cutscene")]
    public GameObject hordaFalsa;

    [Header("O Roteiro da Cena")]
    public DialogueLine[] lines;

    // O Dicionário acha qualquer linha da história instantaneamente usando o ID
    private Dictionary<string, DialogueLine> roteiroDatabase;
    private DialogueLine currentLine;
    private bool isWaitingForChoice = false;

    private void Start()
    {
        // Transforma o roteiro linear num mapa navegável (o Dicionário)
        roteiroDatabase = new Dictionary<string, DialogueLine>();
        foreach (DialogueLine line in lines)
        {
            if (!string.IsNullOrEmpty(line.nodeId) && !roteiroDatabase.ContainsKey(line.nodeId))
            {
                roteiroDatabase.Add(line.nodeId, line);
            }
        }

        // --- SISTEMA DE INÍCIO INTELIGENTE ---
        if (autoStart)
        {
            // Se você digitou algum ID, ele inicia do jeito novo (Twine)
            if (!string.IsNullOrEmpty(startNodeId))
            {
                StartDialogue(startNodeId);
            }
            // Se deixou o campo de texto vazio, ele inicia do jeito antigo (Linear / Fala 0)
            else
            {
                StartDialogue();
            }
        }
    }

    private void Update()
    {
        // Se a cutscene estiver aberta e o jogo pausado...
        if (Time.timeScale == 0f && dialoguePanel.activeSelf)
        {
            // O jogador só pode clicar na tela se NÃO houver uma escolha para ser feita
            if (!isWaitingForChoice && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                AdvanceDialogue();
            }
        }
    }

    // Agora o diálogo sempre começa chamando uma Passagem específica!
    public void StartDialogue(string startNodeId)
    {
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;

        JumpToNode(startNodeId);
    }

    // --- NOVO: Função de retrocompatibilidade para cutscenes antigas ---
    // Se você chamar a função sem passar um ID (ou ligar no botão antigo), ela começa da linha 0.
    public void StartDialogue()
    {
        if (lines.Length > 0)
        {
            dialoguePanel.SetActive(true);
            Time.timeScale = 0f;

            // Ignora o Dicionário e simplesmente puxa a primeira fala da lista do Inspector
            currentLine = lines[0];
            DisplaySentence(currentLine);
        }
    }

    // Pula direto para a caixa solicitada (A mágica do Twine!)
    public void JumpToNode(string nodeId)
    {
        if (roteiroDatabase.TryGetValue(nodeId, out DialogueLine nextLine))
        {
            currentLine = nextLine;
            DisplaySentence(currentLine);
        }
        else
        {
            Debug.LogWarning($"Passagem '{nodeId}' não foi encontrada! O diálogo terminou por segurança.");
            EndDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        // Acha qual é o índice atual da fala para sabermos qual a próxima
        int currentIndex = System.Array.IndexOf(lines, currentLine);

        // Se houver uma próxima fala na lista, avança normalmente
        if (currentIndex >= 0 && currentIndex < lines.Length - 1)
        {
            currentLine = lines[currentIndex + 1];
            DisplaySentence(currentLine);
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplaySentence(DialogueLine line)
    {
        // 1. Limpa as opções velhas da tela antes de mostrar a fala nova
        foreach (Transform child in choicesPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. Sistema de Câmera Modular (Intacto e Blindado!)
        if (line.cameraDaFala != null)
        {
            if (cameraAtualDaCutscene != null) cameraAtualDaCutscene.gameObject.SetActive(false);
            else if (playerCam != null) playerCam.gameObject.SetActive(false); 

            line.cameraDaFala.gameObject.SetActive(true);
            cameraAtualDaCutscene = line.cameraDaFala;
        }

        // 3. Textos e Retratos
        dialogueText.SetText(line.sentence);

        if (string.IsNullOrEmpty(line.characterName)) nameText.gameObject.SetActive(false);
        else { nameText.gameObject.SetActive(true); nameText.SetText(line.characterName); }

        if (line.portrait == null) portraitImage.gameObject.SetActive(false);
        else { portraitImage.gameObject.SetActive(true); portraitImage.sprite = line.portrait; }

        // ==========================================
        // --- 4. NOVO: GERAÇÃO DE BIFURCAÇÕES ---
        // ==========================================
        if (line.choices != null && line.choices.Length > 0)
        {
            // Trava o avanço por clique no espaço/mouse! O jogador TEM que clicar no botão.
            isWaitingForChoice = true;

            foreach (DialogueChoice choice in line.choices)
            {
                // Instancia o botão na tela
                GameObject btnObj = Instantiate(choiceButtonPrefab, choicesPanel);
                btnObj.SetActive(true);

                // Escreve o texto que o designer pediu no botão
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.textDaEscolha;

                // Conecta a lógica de pular para a próxima passagem quando clicar
                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    isWaitingForChoice = false; // Destrava a narrativa
                    JumpToNode(choice.proximoNodeId); // Pula pro destino
                });
            }
        }
        else
        {
            // Se não tiver escolhas, destrava para o jogador poder avançar clicando em qualquer lugar
            isWaitingForChoice = false;
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (hordaFalsa != null) hordaFalsa.SetActive(false);

        if (cameraAtualDaCutscene != null) cameraAtualDaCutscene.gameObject.SetActive(false);

        if (playerCam != null) playerCam.gameObject.SetActive(true); // <--- ESCUDO AQUI

        cameraAtualDaCutscene = null;

        StartCoroutine(ResumeTimeNextFrame());
    }

    private IEnumerator ResumeTimeNextFrame()
    {
        yield return null;
        Time.timeScale = 1f;
    }
}