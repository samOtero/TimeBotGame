using UnityEngine;

public class LevelEdgeMarker : MonoBehaviour
{
    public FloatVariable XLoc;
    public FloatVariable YLoc;

    // Start is called before the first frame update
    void Start()
    {
        XLoc.Value = gameObject.transform.position.x;
        YLoc.Value = gameObject.transform.position.y;
    }
}
