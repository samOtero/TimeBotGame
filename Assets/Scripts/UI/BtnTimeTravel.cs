using UnityEngine;

public class BtnTimeTravel : MonoBehaviour
{
    public GameObject btnGameObject;
    public IntVariable HideTimeTravel;
    private bool didCheck;
    // Start is called before the first frame update
    void Start()
    {
        didCheck = false;
    }

    void Update()
    {
        if (didCheck == false)
        {
            didCheck = true;
            if (HideTimeTravel.Value != 0)
                btnGameObject.SetActive(false);
            else
                btnGameObject.SetActive(true);
        }
    }
}
