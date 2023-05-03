using System.Collections.Generic;
using System.IO;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetector : MonoBehaviour
{
    public int detectionWindowSize = 100; // Number of detections in the window (e.g., 100)
    public float detectionThreshold = 0.8f; // Percentage of positive detections to start the game (e.g., 80%)
    
    private readonly Queue<bool> _detectionWindow = new(); // Queue to store detection results
    private int _positiveDetections = 0; // Counter for positive detections in the window

    // Start is called before the first frame update
    private WebCamTexture _webCamTexture;

    private CascadeClassifier _cascade;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        _cascade =
            new CascadeClassifier(Path
                .Combine(Application.dataPath, "haarcascade_upperbody.xml"));
    }

    // Update is called once per frame
    void Update()
    {
        var frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);
        
        var isBodyDetected = _cascade.DetectMultiScale(frame, 1.04, 5, HaarDetectionType.ScaleImage, new Size(200, 200)).Length > 0;

        if (_detectionWindow.Count >= detectionWindowSize)
        {
            var oldDetection = _detectionWindow.Dequeue();
            if (oldDetection) _positiveDetections--;
        }
        _detectionWindow.Enqueue(isBodyDetected);
        if (isBodyDetected) _positiveDetections++;

        // If a player is detected over the threshold, start the game
        var positiveDetections = _positiveDetections / (float)detectionWindowSize;
        Debug.Log(positiveDetections);
        if (positiveDetections >= detectionThreshold)
        {
            StartGame();
        }
    }

    private static void StartGame()
    {
        if (PlayerData.instance.ftueLevel == 0)
        {
            PlayerData.instance.ftueLevel = 1;
            PlayerData.instance.Save();
        }

        SceneManager.LoadScene("Main");
    }
}