using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenCvSharp;
using UnityEngine;

public class BodyDetector : MonoBehaviour
{
    // Start is called before the first frame update
    private WebCamTexture _webCamTexture;

    private Mat _frame;
    private Texture _recTexture;

    private CascadeClassifier cascade;

    private float mLastBodyX = 0f;

    private OpenCvSharp.Rect mLastBodyRect;

    // Variables to store OpenCV objects
    private Mat mRgba;

    // Variables to store movement direction
    public int mMovementDirection = 0; // 0 = no movement, -1 = left, 1 = right


    public int detectionWindowSize = 100; // Number of detections in the window (e.g., 100)
    public float detectionThreshold = 0.8f; // Percentage of positive detections to start the game (e.g., 80%)

    private readonly Queue<bool> _detectionWindow = new(); // Queue to store detection results
    private int _positiveDetections = 0; // Counter for positive detections in the window
    private static bool _detectionOverThreshhold = false;
    private static float _detectionPercent = 0.0f;

    void Start()
    {
        // Get webcam devices
        WebCamDevice[] devices = WebCamTexture.devices;
        
        // Check if there are any devices
        if (devices.Length == 0) ExitWithError("No webcams found");

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        var cvModel = Path.Combine(Application.streamingAssetsPath, "CVModels/haarcascade_upperbody.xml");
        
        // Check if the model exists
        if (!File.Exists(cvModel)) ExitWithError("CV model not found: " + cvModel);
        
        cascade = new CascadeClassifier(cvModel);
    }

    private static void ExitWithError(string errorMessage)
    {
        Debug.LogError(errorMessage);
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        _frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);
        findNewBody();
        Display();
        GC.Collect();
    }

    void findNewBody()
    {
        var bodies = cascade.DetectMultiScale(_frame, 1.04, 5, HaarDetectionType.ScaleImage, new Size(200, 200));
        // Track the movement of the body
        var isBodyDetected = bodies.Length > 0;
        if (isBodyDetected)
        {
            OpenCvSharp.Rect bodyRect = new OpenCvSharp.Rect();
            // Get the largest body in the frame
            /*
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
            } */
            bodyRect = bodies[0];
            // Track the movement of the body
            if (mLastBodyRect != null)
            {
                // Detect the direction of the body movement
                float bodyX = bodyRect.Location.X; // + bodyRect.Size.Width;
                if (bodyX < 80)
                {
                    mMovementDirection = 2;
                }
                else if (bodyX >= 90 && bodyX <= 210)
                {
                    mMovementDirection = 1;
                }
                else
                {
                    mMovementDirection = 0;
                }
            }
            //Debug.Log("Location: "+bodyRect.Location);
            //Debug.Log("Size: "+bodyRect.Size);

            // Save the current body position and direction for comparison in the next frame
            mLastBodyRect = bodyRect;
            mLastBodyX =
                mLastBodyRect.Location.X + (mLastBodyRect.Size.Width / 2);
        }

        if (_detectionWindow.Count >= detectionWindowSize)
        {
            var oldDetection = _detectionWindow.Dequeue();
            if (oldDetection) _positiveDetections--;
        }

        _detectionWindow.Enqueue(isBodyDetected);
        if (isBodyDetected) _positiveDetections++;

        // If a player is detected over the threshold, start the game
        _detectionPercent = _positiveDetections / (float)detectionWindowSize;
        _detectionOverThreshhold = _detectionPercent > detectionThreshold;
    }

    private void Display()
    {
        if (mLastBodyRect != null)
        {
            _frame.Rectangle(mLastBodyRect, new Scalar(250, 0, 0), 2);
        }

        _recTexture = OpenCvSharp.Unity.MatToTexture(_frame);
        GetComponent<Renderer>().material.mainTexture = _recTexture;
    }

    public static bool GetDetectionOverThreshold()
    {
        return _detectionOverThreshhold;
    }

    public static float GetDetectionPercent()
    {
        return _detectionPercent;
    }
}