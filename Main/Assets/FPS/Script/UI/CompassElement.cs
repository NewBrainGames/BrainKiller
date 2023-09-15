using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassElement : MonoBehaviour
{
    [Tooltip("The marker on the compass for this element")]
    public CompassMarker compassMarkerPrefab;

    [Tooltip("Text override for the marker,if it's a direction")]
    public string textDirection;

    private Compass m_Compass;
    // Start is called before the first frame update
    void Awake()
    {
        m_Compass = FindObjectOfType<Compass>();
        // DebugUtility.HandleErrorIfNullFindObject<Compass, CompassElement>(m_Compass, this);
        var markerInstance = Instantiate(compassMarkerPrefab);
        markerInstance.Initialzie(this, textDirection);
        m_Compass.RegisterCompassElement(transform, markerInstance);

    }

    // Update is called once per frame
    void OnDestory()
    {
        m_Compass.UnregisterCompassElement(transform);
    }
}
