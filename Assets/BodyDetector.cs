using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;

public class BodyDetector : MonoBehaviour
{
    // Start is called before the first frame update
    WebCamTexture _webCamTexture;

    CascadeClassifier cascade;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        cascade =
            new CascadeClassifier(System
                    .IO
                    .Path
                    .Combine(Application.dataPath, "haarcascade_fullbody.xml"));
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);
        findNewFace (frame);
    }

    void findNewFace(Mat frame)
    {
        var faces =
            cascade
                .DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);

        if (faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
        }
    }
}
