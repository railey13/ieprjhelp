using UnityEngine;

public class EnemyIntentDisplay : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color pathColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    [SerializeField] private Color stopColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
    [SerializeField] private Color rangeColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

    [Header("Sizes")]
    [SerializeField] private float pathWidth = 0.4f;
    [SerializeField] private float stopCircleRadius = 0.4f;
    private GameObject targetLight;
    private GameObject pathBox;
    private GameObject stopCircle;
    private GameObject rangeCircle;
    private bool isVisible = false;

    void Awake()
    {
        BuildVisuals();
        SetVisible(false);
    }

    void BuildVisuals()
    {
        // path enemies will take
        pathBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pathBox.name = "IntentPath_" + gameObject.name;
        DestroyImmediate(pathBox.GetComponent<Collider>());
        ApplyMaterial(pathBox, pathColor);

        // WHERE enemy will stop
        stopCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stopCircle.name = "IntentStop_" + gameObject.name;
        DestroyImmediate(stopCircle.GetComponent<Collider>());
        stopCircle.transform.localScale = new Vector3(stopCircleRadius * 2f, 0.02f, stopCircleRadius * 2f);
        ApplyMaterial(stopCircle, stopColor);

        // enemy's range BASED on where they stop
        rangeCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rangeCircle.name = "IntentRange_" + gameObject.name;
        DestroyImmediate(rangeCircle.GetComponent<Collider>());
        ApplyMaterial(rangeCircle, rangeColor);

        targetLight = new GameObject("IntentLight_" + gameObject.name);
        Light lightComp = targetLight.AddComponent<Light>();
        lightComp.type = LightType.Spot;
        lightComp.color = Color.red;
        lightComp.range = 5f;
        lightComp.intensity = 200f;
        lightComp.spotAngle = 60f;
    }

    void ApplyMaterial(GameObject obj, Color color)
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = color;
        obj.GetComponent<MeshRenderer>().material = mat;
    }

    public void UpdateIntent(Vector3 destination, float enemyRange)
    {
        Vector3 from = transform.position;
        Vector3 to = destination;
        to.y = from.y; // keep flat

        // path stretches between from and to
        Vector3 midpoint = (from + to) / 2f;
        float distance = Vector3.Distance(from, to);

        pathBox.transform.position = new Vector3(midpoint.x, from.y + 0.01f, midpoint.z);

        if (distance > 0.01f)
        {
            Vector3 direction = (to - from).normalized;
            pathBox.transform.rotation = Quaternion.LookRotation(direction);
            pathBox.transform.localScale = new Vector3(pathWidth, 0.02f, distance);
            pathBox.SetActive(isVisible);
        }
        else
        {
            pathBox.SetActive(false);
        }

        // place stop circle  at destination
        stopCircle.transform.position = new Vector3(to.x, from.y + 0.05f, to.z);
        stopCircle.transform.rotation = Quaternion.identity;

        // range circle placed where enemy would stand
        rangeCircle.transform.position = new Vector3(to.x, from.y + 0.01f, to.z);
        rangeCircle.transform.localScale = new Vector3(enemyRange * 2f, 0.02f, enemyRange * 2f);
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (pathBox != null) pathBox.SetActive(visible);
        if (stopCircle != null) stopCircle.SetActive(visible);
        if (rangeCircle != null) rangeCircle.SetActive(visible);
        if (targetLight != null) targetLight.SetActive(visible);
    }

    public bool IsVisible()
    {
        return isVisible;
    }

    void OnDestroy()
    {
        if (pathBox != null) Destroy(pathBox);
        if (stopCircle != null) Destroy(stopCircle);
        if (rangeCircle != null) Destroy(rangeCircle);
    }

    public void UpdateTargetLight(Transform target)
    {
        if (target == null)
        {
            targetLight.SetActive(false);
            return;
        }

        targetLight.SetActive(isVisible);

        targetLight.transform.position =
            target.position + Vector3.up * 1.3f;

        targetLight.transform.forward = Vector3.down;
    }
}