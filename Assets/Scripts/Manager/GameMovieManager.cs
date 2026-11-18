using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMovieManager : Singleton<GameMovieManager>
{
    RawImage rawImage;
    VideoPlayer videoPlayer;
    RenderTexture originalMovieTarget, titleMovieTarget;
    Text text;

    OnEventOver onEventOver;

    bool isPlaying;
    bool tracingStartup, preparingStartup, startupFrameReady, startupFailed, startupDecodedFrame;
    Coroutine startupRoutine;
    public bool IsPlaying { get { return isPlaying; } }

    public override void initSingleton()
    {
        rawImage = transform.Find( "Movie" ).GetComponent<RawImage>();
        videoPlayer = transform.Find( "Movie" ).GetComponent<VideoPlayer>();
        originalMovieTarget = videoPlayer.targetTexture;
        videoPlayer.loopPointReached += onPlayOver;

        rawImage.enabled = false;

        text = transform.Find( "Text" ).GetComponent< Text >();
        text.gameObject.SetActive( false );

        gameObject.SetActive( false );
    }

    void SelectMovieRenderTarget( string path )
    {
        if ( path == "Movie/Title" || path == "Movie/Movie_00" )
        {
            if ( titleMovieTarget == null )
            {
                titleMovieTarget = new RenderTexture( 1280, 800, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
                titleMovieTarget.name = "TitleMovieRenderTextureHD";
                titleMovieTarget.filterMode = originalMovieTarget.filterMode;
                titleMovieTarget.wrapMode = originalMovieTarget.wrapMode;
            }
            if ( !titleMovieTarget.IsCreated() ) titleMovieTarget.Create();
            videoPlayer.targetTexture = titleMovieTarget;
            rawImage.texture = titleMovieTarget;
        }
        else
        {
            videoPlayer.targetTexture = originalMovieTarget;
            rawImage.texture = originalMovieTarget;
        }
    }

    void OnDestroy()
    {
        if ( titleMovieTarget == null ) return;
        titleMovieTarget.Release();
        Destroy( titleMovieTarget );
    }

    public void playMovieCenter( string str , OnEventOver over )
    {
        RectTransform trans = GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2( 0.0f , 0.0f );

        videoPlayer.Stop();
        SelectMovieRenderTarget( str );

        onEventOver = over;

        gameObject.SetActive( true );

        if ( str == "Movie/Title" && SwordsmanStartupPrototype.Tracing ) BeginStartupTrace();
        VideoClip clip = Resources.Load<VideoClip>( str );
        if ( tracingStartup ) SwordsmanStartupPrototype.Mark( "video_resource_loaded" );
        videoPlayer.clip = clip;
        videoPlayer.Play();

        rawImage.enabled = true;

        isPlaying = true;

#if UNITY_EDITOR
        Debug.Log( "playMovieCenter " + str + " " + clip );
#endif
    }

    // Static startup cover; story/event video playback keeps its original path.
    public void playStartupPrototype( string path, OnEventOver over )
    {
        if ( startupRoutine != null ) { StopCoroutine( startupRoutine ); startupRoutine = null; }
        SwordsmanStartupPrototype.BeginVideo();
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        videoPlayer.Stop(); onEventOver = over;
        SelectMovieRenderTarget( path );
        gameObject.SetActive( true ); rawImage.enabled = false;
        isPlaying = true; preparingStartup = true; startupFrameReady = false; startupFailed = false;
        BeginStartupTrace();
        startupRoutine = StartCoroutine( PrepareStartup( path ) );
    }

    void BeginStartupTrace()
    {
        EndStartupTrace();
        tracingStartup = true;
        startupDecodedFrame = false;
        SwordsmanStartupPrototype.Mark( "video_load_begin" );
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += StartupFrame;
        videoPlayer.errorReceived += StartupError;
        videoPlayer.prepareCompleted += StartupPrepared;
    }
    void EndStartupTrace()
    {
        if ( !tracingStartup ) return;
        videoPlayer.sendFrameReadyEvents = false;
        videoPlayer.frameReady -= StartupFrame;
        videoPlayer.errorReceived -= StartupError;
        videoPlayer.prepareCompleted -= StartupPrepared;
        tracingStartup = false;
    }

    void StartupPrepared( VideoPlayer player ) { SwordsmanStartupPrototype.Mark( "video_prepared" ); }
    void StartupError( VideoPlayer player, string message )
    {
        startupFailed = true;
        SwordsmanStartupPrototype.Mark( "video_error" );
    }
    void StartupFrame( VideoPlayer player, long frame )
    {
        if ( !startupDecodedFrame )
        {
            startupDecodedFrame = true;
            SwordsmanStartupPrototype.Mark( "video_first_frame_ready" );
        }
        // The replacement Movie/Title has visible star-light in frame 0.
        // The cover still remains until a completed video render below.
        startupFrameReady = true;
        if ( preparingStartup ) rawImage.enabled = true;
        SwordsmanStartupPrototype.Mark( "video_first_content_frame_ready" );
        player.sendFrameReadyEvents = false;
        player.frameReady -= StartupFrame;
    }

    IEnumerator PrepareStartup( string path )
    {
        // Let the logo reach a render opportunity before synchronous resource loading.
        yield return null;
        videoPlayer.clip = Resources.Load<VideoClip>( path );
        SwordsmanStartupPrototype.Mark( "video_resource_loaded" );
        if ( videoPlayer.clip == null )
        {
            SwordsmanStartupPrototype.Mark( "video_missing" ); stopMovie(); yield break;
        }
        videoPlayer.Prepare();
        float deadline = Time.realtimeSinceStartup + 15f;
        while ( !videoPlayer.isPrepared && !startupFailed && Time.realtimeSinceStartup < deadline ) yield return null;
        if ( startupFailed || !videoPlayer.isPrepared )
        {
            SwordsmanStartupPrototype.Mark( "video_prepare_failed_or_timed_out" ); stopMovie(); yield break;
        }
        videoPlayer.Play();
        SwordsmanStartupPrototype.Mark( "video_play_requested" );
        deadline = Time.realtimeSinceStartup + 15f;
        while ( !startupFrameReady && !startupFailed && Time.realtimeSinceStartup < deadline ) yield return null;
        if ( startupFailed || !startupFrameReady )
        {
            SwordsmanStartupPrototype.Mark( "video_first_frame_failed_or_timed_out" ); stopMovie(); yield break;
        }
        // Keep the cover through one completed video render before revealing it.
        if ( Application.isBatchMode ) yield return null;
        else yield return new WaitForEndOfFrame();
        SwordsmanStartupPrototype.Finish( "logo_hidden_after_video_render" );
        preparingStartup = false; startupRoutine = null;
    }

    public void playMovie( string str , OnEventOver over )
    {
        RectTransform trans = GetComponent<RectTransform>();
//        trans.anchoredPosition = new Vector2( GameCameraManager.instance.xOffset , 0.0f );

        videoPlayer.Stop();
        SelectMovieRenderTarget( str );

        onEventOver = over;

        gameObject.SetActive( true );

        VideoClip clip = Resources.Load<VideoClip>( str );
        videoPlayer.clip = clip;
        videoPlayer.Play();

        rawImage.enabled = true;

        isPlaying = true;

#if UNITY_EDITOR
        Debug.Log( "playMovie " + str + " " + clip );
#endif
    }

    public void onClick()
    {
        if ( text.gameObject.activeSelf )
        {
            stopMovie();
        }
        else
        {
            text.gameObject.SetActive( true );
        }
    }

    public void stopMovie()
    {
        isPlaying = false;
        if ( startupRoutine != null ) { StopCoroutine( startupRoutine ); startupRoutine = null; }
        EndStartupTrace();
        if ( preparingStartup ) SwordsmanStartupPrototype.Finish( "startup_video_stopped" );
        preparingStartup = false;

        videoPlayer.Stop();
        rawImage.enabled = false;
        text.gameObject.SetActive( false );

        gameObject.SetActive( false );

        if ( onEventOver != null )
        {
            onEventOver();
        }

        if ( !isPlaying )
        {
            videoPlayer.targetTexture.Release();
        }
    }

    public void onPlayOver( VideoPlayer p )
    {
#if UNITY_EDITOR
        Debug.Log( "onPlayOver" );
#endif
        stopMovie();
    }



}
