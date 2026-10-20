
using UnityEngine;
using UnityEngine.Rendering;

public class VisionCone : MonoBehaviour
{
    public float visionRange = 10f;
    [Range(10f, 180f)]
    public float visionAngle = 90f;

    public Color coneColor = new Color(1f, 0.8f, 0f, 0.3f);
    public float heightOffset = 0.1f;
    [Range(3, 100)]
    public int segments = 40;

    Mesh mesh;
    MeshFilter meshFilter;
    Material runtimeMaterial;

    void Start()
    {
        
        GameObject visual = new GameObject("VisionConeVisual");
        visual.transform.SetParent(transform, false);
        visual.transform.localPosition = new Vector3(0, heightOffset, 0);
        visual.transform.localRotation = Quaternion.identity;

        meshFilter = visual.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = visual.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "VisionConeMesh";
        meshFilter.mesh = mesh;

        
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError("No se encontró un shader compatible.");
            return;
        }

        runtimeMaterial = new Material(shader);
        runtimeMaterial.color = coneColor;

       
        if (runtimeMaterial.HasProperty("_Surface"))
            runtimeMaterial.SetFloat("_Surface", 1f);

        if (runtimeMaterial.HasProperty("_BaseColor"))
            runtimeMaterial.SetColor("_BaseColor", coneColor);

        runtimeMaterial.SetInt("_SrcBlend",
            (int)BlendMode.SrcAlpha);
        runtimeMaterial.SetInt("_DstBlend",
            (int)BlendMode.OneMinusSrcAlpha);
        runtimeMaterial.SetInt("_ZWrite", 0);
        runtimeMaterial.renderQueue = 3000;

        runtimeMaterial.SetInt("_Cull", (int)CullMode.Off);

        meshRenderer.material = runtimeMaterial;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        UpdateCone();
        Debug.Log("VisionCone: abanico creado.", this);
    }

    void Update()
    {
        if (mesh != null)
            UpdateCone();
    }

    void UpdateCone()
    {
        int count = Mathf.Max(3, segments);

        Vector3[] vertices = new Vector3[count + 2];
        int[] triangles = new int[count * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= count; i++)
        {
            float angle = (-visionAngle / 2f
                + visionAngle * i / count) * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(angle) * visionRange,
                0f,
                Mathf.Cos(angle) * visionRange
            );

            if (i < count)
            {
                int t = i * 3;
                triangles[t] = 0;
                triangles[t + 1] = i + 1;
                triangles[t + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void OnDestroy()
    {
        if (mesh != null)
            Destroy(mesh);

        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
