using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureView : MonoBehaviour
{

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CamCapture();
        }
    }

    void CamCapture()
    {
        ScreenCapture.CaptureScreenshot("Assets/Output/a_temp.png");
    }

}
