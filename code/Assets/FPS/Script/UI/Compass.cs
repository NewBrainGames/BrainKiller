using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    
    public RectTransform compasRect;
    public float visibilityAngle = 180f;

    public float minScale=0.5f;

    public float heightDifferenceMultiplier;

    public float distanceMinScale = 50f;
    public float compasMarginRatio = 0.8f;
    public GameObject MarkerDirectionPrefab;

    private Transform m_PlayerTransform;
    private Dictionary<Transform, CompassMarker> m_ElementDictionnary = new Dictionary<Transform, CompassMarker>();
    private float m_widthMultiplier;
    private float m_heightOffset;
    
    // Start is called before the first frame update

    void Awake()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        // DebugUtility.HandleErrorIfNullFindObject<PlayerController, Compass>(playerController, this);
        m_PlayerTransform = playerController.transform;
        m_widthMultiplier = compasRect.rect.width / visibilityAngle;
        m_heightOffset = -compasRect.rect.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var element in m_ElementDictionnary)
        {
            float distanceRatio = 1;
            float heightDifference = 0;
            float angle;
            if (element.Value.isDirection)
            {
                angle = Vector3.SignedAngle(m_PlayerTransform.forward, element.Key.transform.localPosition.normalized,
                    Vector3.up);
            }
            else
            {
                Vector3 targetDir = (element.Key.transform.position - m_PlayerTransform.position).normalized;
                targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);
                Vector3 playerForward = Vector3.ProjectOnPlane(m_PlayerTransform.forward,Vector3.up);
                angle = Vector3.SignedAngle(playerForward, targetDir, Vector3.up);

                Vector3 directionVector = element.Key.transform.position - m_PlayerTransform.position;
                heightDifference = (directionVector.y) * heightDifferenceMultiplier;
                heightDifference = Mathf.Clamp(heightDifference, -compasRect.rect.height / 2 * compasMarginRatio,
                    compasRect.rect.height / 2 * compasMarginRatio);
                distanceRatio = directionVector.magnitude / distanceMinScale;
                distanceRatio = Mathf.Clamp01(distanceRatio);
            }

            if (angle > -visibilityAngle / 2 && angle < visibilityAngle / 2)
            {
                element.Value.canvasGroup.alpha = 1;
                element.Value.canvasGroup.transform.localPosition =
                    new Vector2(m_widthMultiplier * angle, heightDifference + m_heightOffset);
                element.Value.canvasGroup.transform.localScale = Vector3.one * Mathf.Lerp(1, minScale, distanceRatio);
            }
            else
            {
                element.Value.canvasGroup.alpha = 0;
            }
            

        }
    }

    public void RegisterCompassElement(Transform element, CompassMarker marker)
    {
        marker.transform.SetParent(compasRect);
        m_ElementDictionnary.Add(element, marker);
    }

    public void UnregisterCompassElement(Transform element)
    {
        if (m_ElementDictionnary.TryGetValue(element, out CompassMarker marker) && marker.canvasGroup != null)
            Destroy(marker.canvasGroup.gameObject);
        m_ElementDictionnary.Remove(element);
    }

}
