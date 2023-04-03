using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;

public class BodyDetector : MonoBehaviour
{
    // Start is called before the first frame update
    private WebCamTexture _webCamTexture;

    private CascadeClassifier cascade;

    private float mLastBodyX = 0f;

    private OpenCvSharp.Rect mLastBodyRect;

    // Variables to store OpenCV objects
    private Mat mRgba;

    // Variables to store movement direction
    public int mMovementDirection = 0; // 0 = no movement, -1 = left, 1 = right

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        cascade =
            new CascadeClassifier(System
                    .IO
                    .Path
                    .Combine(Application.dataPath, "haarcascade_upperbody.xml"));
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
        var bodies = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
        // Track the movement of the body
        if (bodies.Length > 0)
        {
            // Get the largest body in the frame
            OpenCvSharp.Rect bodyRect = new OpenCvSharp.Rect();
            foreach (OpenCvSharp.Rect body in bodies)
            {
                if (
                    bodyRect == null ||
                    body.Size.Height * body.Size.Width >
                    bodyRect.Size.Height * body.Size.Width
                )
                {
                    bodyRect = body;
                }
            }

            // Track the movement of the body
            if (mLastBodyRect != null)
            {
                // Detect the direction of the body movement
                float bodyX = bodyRect.Location.X;// + bodyRect.Size.Width;
                if (bodyX < 150)
                {
                    mMovementDirection = 2;
                }
                else if (bodyX >= 160 && bodyX <= 230)
                {
                    mMovementDirection = 1;
                }
                else
                {
                    mMovementDirection = 0;
                }

                // TODO: Trigger an event in the game based on the direction of the body movement (e.g. play a different animation or adjust the speed of the game)
            }
            Debug.Log("Location: "+bodyRect.Location);
            Debug.Log("Size: "+bodyRect.Size);

            // Save the current body position and direction for comparison in the next frame
            mLastBodyRect = bodyRect;
            mLastBodyX =
                mLastBodyRect.Location.X + (mLastBodyRect.Size.Width / 2);
        }
    }
}
