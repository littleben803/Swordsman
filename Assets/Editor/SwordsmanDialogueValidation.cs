using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SwordsmanDialogueValidation
{
    const string Key = "Swordsman.DialogueSpeed.Check";
    const string Output = "Logs/dialogue-speed-20261116";
    static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
    static SwordsmanDialogueVerification.Preferences prefs;
    static double deadline;
    [Serializable] sealed class Report { public List<string> passed = new List<string>(); public string error = ""; }
    static Report report;
    static void View( bool enabled )
    {
        typeof(SwordsmanVirtualKeyboardSkinValidation).GetMethod( "ConfigureGameView", BindingFlags.Static | BindingFlags.NonPublic ).Invoke( null, new object[] { enabled } );
    }
    public static void Run()
    {
        Directory.CreateDirectory( Output );
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        View( true ); SessionState.SetBool( Key, true ); EditorApplication.EnterPlaymode();
    }
    static IEnumerator Load( int scene )
    {
        string path = "Assets/Scenes/level" + scene + ".unity";
        EditorSceneManager.LoadSceneInPlayMode( path, new LoadSceneParameters( LoadSceneMode.Single ) );
        while ( SceneManager.GetActiveScene().path != path ) yield return null;
        yield return null; yield return null;
    }
    [InitializeOnLoadMethod] static void Resume()
    {
        if ( !SessionState.GetBool( Key, false ) ) return;
        report = new Report(); prefs = new SwordsmanDialogueVerification.Preferences();
        deadline = EditorApplication.timeSinceStartup + 120; stack.Clear();
        stack.Push( SwordsmanDialogueVerification.Run( (ok, name) => { if ( !ok ) throw new Exception( name ); report.passed.Add( name ); },
            name => ScreenCapture.CaptureScreenshot( Path.GetFullPath( Output + "/" + name + ".png" ) ), Load ) );
        EditorApplication.update += Tick;
    }
    static void Tick()
    {
        if ( !EditorApplication.isPlaying ) return;
        try
        {
            if ( EditorApplication.timeSinceStartup > deadline ) throw new Exception( "Dialogue check timeout" );
            while ( stack.Count > 0 )
            {
                if ( !stack.Peek().MoveNext() ) { stack.Pop(); continue; }
                object current = stack.Peek().Current;
                if ( current is IEnumerator ) { stack.Push( (IEnumerator)current ); continue; }
                return;
            }
            Finish( "" );
        }
        catch ( Exception e ) { Finish( e.ToString() ); }
    }
    static void Finish( string error )
    {
        EditorApplication.update -= Tick; SessionState.SetBool( Key, false );
        prefs.Restore(); report.error = error;
        try { View( false ); } catch ( Exception e ) { report.error += e; }
        File.WriteAllText( Output + "/editor-check.json", JsonUtility.ToJson( report, true ) );
        EditorApplication.Exit( report.error.Length == 0 ? 0 : 1 );
    }
}
