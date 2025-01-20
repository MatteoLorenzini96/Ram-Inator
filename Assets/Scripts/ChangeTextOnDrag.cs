using UnityEngine;
using TMPro;

public class ChangeTextOnDrag : MonoBehaviour
{
    // Riferimento al TextMeshPro da modificare
    public TextMeshProUGUI textMeshPro;

    // Riferimento al componente WreckingBallDrag
    private WreckingBallDrag wreckingBallDrag;

    private void Start()
    {
        // Trova lo script WreckingBallDrag nella scena
        wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();

        // Verifica se il componente è stato trovato
        if (wreckingBallDrag == null)
        {
            Debug.LogError("Non è stato trovato lo script WreckingBallDrag nella scena!");
        }

        // Verifica se il TextMeshPro è stato assegnato
        if (textMeshPro == null)
        {
            Debug.LogError("Il componente TextMeshProUGUI non è stato assegnato!");
        }
    }

    private void Update()
    {
        // Controlla se la variabile isDragging è vera
        if (wreckingBallDrag != null && wreckingBallDrag.isDragging)
        {
            // Modifica il testo del TextMeshPro
            textMeshPro.text = "When it's ready swipe to destroy";
        }
        else
        {
            // Modifica il testo quando non sta trascinando
            textMeshPro.text = "Move the ball and let it charge";
        }
    }
}
