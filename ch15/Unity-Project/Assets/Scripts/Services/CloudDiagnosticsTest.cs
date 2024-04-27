using System;
using UnityEngine;

public class CloudDiagnosticsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogException(
            new Exception(("Cloud diagnostics test!")));
    }
}
