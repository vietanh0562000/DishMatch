using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoWithThumbnail : MonoBehaviour
{
	public Image       thumbnailImage;
	public VideoPlayer videoPlayer;

	void OnEnable()
	{
		videoPlayer.Prepare();

		videoPlayer.prepareCompleted += OnVideoReady;
	}

	void OnVideoReady(VideoPlayer vp)
	{
		videoPlayer.Play();

		if (thumbnailImage != null)
		{
			thumbnailImage.enabled = false;
		}
	}

	private void OnDisable()
	{
		thumbnailImage.enabled       =  true;
		videoPlayer.prepareCompleted -= OnVideoReady;
	}
}