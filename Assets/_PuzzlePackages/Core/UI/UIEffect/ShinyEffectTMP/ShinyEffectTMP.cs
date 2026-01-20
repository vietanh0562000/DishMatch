using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public abstract class ShinyEffectTMP : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("EDITOR")]
    [SerializeField] private bool debug = false;
    [Space(5)]
#endif
    [Header("PROPS")]
    public TextMeshProUGUI textMeshPro;

    [Header("RUNTIME PROPS")] 
    [Tooltip("Tốc độ Shiny chaỵ qua từng chữ cái, không phụ thuộc vào độ dài của text.")]
    public float absoluteSpeed = 10f;

    [Tooltip("Thời gian delay giữa 2 lần Shiny xuất hiện")]
    public float interval = 1.0f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Độ rộng của Shiny dựa trên tỉ lệ với Bounds của TextMeshPro")]
    public float width = 0.5f;
    
    [Space(5)]
    [Tooltip("Sử dụng width dựa trên số lương ký tự thay vì tỷ lệ với Bounds của text")]
    public bool useCharactersWidthInstead = true;
    [Tooltip("Độ rộng của shiny tính theo số ký tự, nên đặt tối thiểu là 3")]
    public int charactersWidth = 5;

    public float Speed
    {
        get
        {
            if (textMeshPro == null || string.IsNullOrWhiteSpace(textMeshPro.text)) return 0;
            return absoluteSpeed / textMeshPro.text.Length;
        }
    }

    public AnimationCurve shinyCurve;
    public Color32 defaultColor;
    public Color32 shinyColor;

    private void Awake()
    {
        var isDuplicated = CheckDuplicateShinyEffectOnGameObject();
        if (isDuplicated)
        {
            Debug.LogWarning("Duplicate shiny effect! Destroy old shiny effect component!");
        }
        
        GetTextMeshPro();
    }

    void LateUpdate()
    {
        if (string.IsNullOrWhiteSpace(textMeshPro.text)) return;
        
        textMeshPro.ForceMeshUpdate();

        float finalWidth = width;
        if (useCharactersWidthInstead)
        {
            if (textMeshPro.text.Length <= charactersWidth) finalWidth = 1.0f;
            else finalWidth = (float)charactersWidth / textMeshPro.text.Length;
        }
        
        var textRect = textMeshPro.textBounds.size;
        float offset = Mathf.Repeat(Time.time * Speed, 1f + 2 * finalWidth + interval * Speed) - finalWidth;

        TMP_TextInfo textInfo = textMeshPro.textInfo;

#if UNITY_EDITOR
        var debugString = $"Offset: {offset:F2}\n";
#endif

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            float startPadding = -vertices[0].x / textRect.x;
            float leftNormPos = vertices[vertexIndex].x / textRect.x + startPadding + finalWidth;
            float rightNormPos = vertices[Mathf.Min(vertexIndex + 3, vertices.Length - 1)].x / textRect.x + startPadding + finalWidth;
            
            float leftValue = Evaluate(finalWidth > 0? (leftNormPos - offset) / finalWidth : 0);
            var colorLeft = CalculateColor32(leftValue);

            float rightValue = Evaluate(finalWidth > 0? (rightNormPos - offset) / finalWidth : 0);
            var colorRight = CalculateColor32(rightValue);
            
#if UNITY_EDITOR
            debugString += $"Index {i}: Padding {startPadding:F2} ---- {leftNormPos:F2} >> {leftValue:F2} ---- {rightNormPos:F2} >> {rightValue:F2}\n";
#endif
            
            vertexColors[vertexIndex + 0] = colorLeft;
            vertexColors[vertexIndex + 1] = colorLeft;
            vertexColors[vertexIndex + 2] = colorRight;
            vertexColors[vertexIndex + 3] = colorRight;
        }
        
#if UNITY_EDITOR
        if (debug) Debug.LogError(debugString);
#endif

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    protected abstract Color32 CalculateColor32(float positionValue);

    /// <summary>
    /// Tắt hiệu ứng và set lại material thành defaultMaterial nếu cờ 'cacheDefaultProperties' được bật.
    /// </summary>
    [Sirenix.OdinInspector.Button]
    public void TurnOff()
    {
        textMeshPro.SetAllDirty();
        enabled = false;
    }

    /// <summary>
    /// Bật lại hiệu ứng shiny
    /// </summary>
    [Sirenix.OdinInspector.Button]
    public void TurnOn()
    {
        enabled = true;
    }

    /// <summary>
    /// Kiểm tra liệu gameObject đã chứa ShinyEffect nào hay chưa, nếu đã có thì Destroy Component Shiny hiện tại và TurnOn Shiny còn lại.
    /// </summary>
    /// <returns>Đã có Shiny Effect trên GameObject hay chưa</returns>
    bool CheckDuplicateShinyEffectOnGameObject()
    {
        bool isDuplicated = false;
        var otherShinyEffectTmp = GetComponents<ShinyEffectTMP>();
        if (otherShinyEffectTmp.Length > 1)
        {
            for (int i = 0; i < otherShinyEffectTmp.Length; i++)
            {
                if (otherShinyEffectTmp[i] != this)
                {
                    otherShinyEffectTmp[i].TurnOff();
                    Destroy(otherShinyEffectTmp[i]);
                    isDuplicated = true;
                }
            }
        }

        return isDuplicated;
    }
    
    float Evaluate(float x)
    {
        return shinyCurve.Evaluate(x);
    }

    void GetTextMeshPro()
    {
        if (!textMeshPro) textMeshPro = GetComponent<TextMeshProUGUI>();
        if (!textMeshPro) textMeshPro = gameObject.AddComponent<TextMeshProUGUI>();
    }

#if UNITY_EDITOR
    private bool editorGetTextMeshPro = true;

    
    
    private void OnValidate()
    {
        if (editorGetTextMeshPro)
        {
            editorGetTextMeshPro = false;
            GetTextMeshPro();
        }
    }
#endif
}