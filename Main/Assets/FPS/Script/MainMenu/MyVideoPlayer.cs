//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Video;
//using UnityEngine.UI;

//public class MyVideoPlayer : MonoBehaviour
//{
//    private VideoPlayer videoPlayer;
//    public VideoClip a;
//    private RawImage rawImage;
//    public float Fadespeed = 1f;

//    private void Awake()
//    {
//        //��ȡCanvals��Ӧ�����
//        videoPlayer = this.GetComponent<VideoPlayer>();
//        rawImage = this.GetComponent<RawImage>();
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        videoPlayer.isLooping = false;
//        videoPlayer.clip = a;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (videoPlayer.texture == null)
//        {
//            return;
//        }

//        //��VideoPlayerd����Ƶ��Ⱦ��UGUI��RawImage
//        rawImage.texture = videoPlayer.texture;
//        VideoFade();
//    }
//    public void VideoFade()
//    {
//        videoPlayer.Play();
//        rawImage.color = Color.Lerp(rawImage.color, Color.white, Fadespeed * Time.deltaTime);

//    }

//}
