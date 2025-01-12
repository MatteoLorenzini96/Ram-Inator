using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TextCurveFromCenter : MonoBehaviour
{
    public float curveHeight = 50f; // Altezza massima della curva al centro
    private TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null)
        {
            Debug.LogError("Questo script richiede un componente TextMeshPro sullo stesso GameObject.");
        }
    }

    void Update()
    {
        if (textMesh != null)
        {
            ApplyCurve();
        }
    }

    private void ApplyCurve()
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;

        // Itera su ogni carattere visibile nel testo
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            // Calcola la posizione normalizzata del carattere (-1 sinistra, 0 centro, +1 destra)
            float normalizedPosition = (charInfo.origin - textMesh.bounds.min.x) / textMesh.bounds.size.x * 2 - 1;

            // Calcola l'altezza usando una parabola: y = -x^2 + 1
            float curveOffsetY = (-Mathf.Pow(normalizedPosition, 2) + 1) * curveHeight;

            // Applica lo spostamento verticale ai vertici del carattere
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j].y += curveOffsetY;
            }
        }

        // Aggiorna il mesh del testo con i nuovi vertici
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
