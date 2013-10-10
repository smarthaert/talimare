///-----------------------------------------------------------------------------------------------
/// author Ivan Murashko iclickable@gmail.com
///-----------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

using UnityDebug = UnityEngine.Debug;
using FrameInfo = Debug.Container.FrameInfo;
using Log = Debug.Container.Log;

public class StackJumper : EditorWindow
{
    #region Menu items
    [MenuItem("Window/Stack Jumper/Show")]
    private static void Init()
    {
        Instance.Show();
    }

    [MenuItem("Window/Stack Jumper/Clear")]
    private static void Clear()
    {
        Debug.Container.Clear();
    }

    [MenuItem("Window/Stack Jumper/Hide")]
    private static void Hide()
    {
        Instance.Close();
    }

    private static StackJumper Instance
    {
        get { return StackJumper.GetWindow<StackJumper>(); }
    }
    #endregion Menu items

    private void OnEnable()
    {
        m_splitter = SplitterBottom - SplitterBottom / 4;
        m_splitterDrag = false;
        wantsMouseMove = true;
        m_selectedMessage = 0;
        m_selectedFrame = 0;

        LoadConsoleData();

        int count = Debug.Container.GetLength();
        m_currentContent = new List<GUIContent>();
        for (int i = 0; i < count; i++)
        {
            Log message = Debug.Container.GetLog(i);
            AddContent(message);
        }

        Debug.Container.OnLogAdded += new System.Action<Log>(AddContent);
        Debug.Container.OnLogsCleared += new System.Action(ClearContent);

    }

    private void LoadConsoleData()
    {
        List<string> assets = AssetDatabase.GetAllAssetPaths().Where(c => c.Contains("ConsoleData/")).ToList();

        m_textureBackground = (Texture2D)AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("background.png")), typeof(Texture2D));
        m_textureError = (Texture2D)AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("error.png")), typeof(Texture2D));
        m_textureLog = (Texture2D)AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("log.png")), typeof(Texture2D));
        m_textureWarning = (Texture2D)AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("warning.png")), typeof(Texture2D));
        m_textureUp = (Texture2D)AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("up.png")), typeof(Texture2D));


        Func<GUISkin, GUIStyle> converter = (GUISkin skin) => { return skin != null ? skin.button : null; };
        m_styleTopFirst = converter(AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("SkinTopFirst.guiskin")), typeof(GUISkin)) as GUISkin);
        m_styleTopSecond = converter(AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("SkinTopSecond.guiskin")), typeof(GUISkin)) as GUISkin);
        m_styleBottomFirst = converter(AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("SkinBottomFirst.guiskin")), typeof(GUISkin)) as GUISkin);
        m_styleBottomSecond = converter(AssetDatabase.LoadAssetAtPath(assets.First(c => c.Contains("SkinBottomSecond.guiskin")), typeof(GUISkin)) as GUISkin);
    }

    private void OnDestroy()
    {
        Debug.Container.OnLogAdded -= AddContent;
        Debug.Container.OnLogsCleared -= ClearContent;
    }

    private void AddContent(Log message)
    {
        Texture2D texture = null;
        switch (message.Type)
        {
            case Debug.LogType.Error:
                texture = m_textureError;
                break;
            case Debug.LogType.Warning:
                texture = m_textureWarning;
                break;
            default:
                texture = m_textureLog;
                break;
        }

        m_currentContent.Add(new GUIContent(message.ToString(), texture));
        Repaint();
    }

    private void ClearContent()
    {
        m_currentContent.Clear();
        Repaint();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), m_textureBackground);

        GUIToolBar();
        GUISplitter();
        GUICallStack();
    }

    private void OnMessageDoubleClick(int index)
    {
        try
        {
            FrameInfo frame = Debug.Container.GetLog(index).Frames.First();
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(frame.FileName, frame.LineNumber);
        }
        catch { }
    }

    private void OnFrameDoubleClick(int index)
    {
        try
        {
            FrameInfo frame = Debug.Container.GetLog(m_selectedMessage).Frames[index];
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(frame.FileName, frame.LineNumber);
        }
        catch { }
    }

    private void GUIToolBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("", "toolbarButton", GUILayout.MaxWidth(5));
        if (GUILayout.Button("Clear", "toolbarButton", GUILayout.Width(50)))
        {
            Debug.Container.Clear();
        }
        GUILayout.Label("", "toolbarButton");
        GUILayout.EndHorizontal();
    }

    private void GUISplitter()
    {
        float dy = 5;
        Rect rect = new Rect(0, m_splitter - dy, position.width, dy * 2);
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.mousePosition.y > m_splitter - dy && Event.current.mousePosition.y < m_splitter + dy * 2)
            {
                m_splitterDrag = true;
            }
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            m_splitterDrag = false;
        }
        else if (Event.current.type == EventType.MouseDrag)
        {
            if (m_splitterDrag)
            {
                if (Event.current.mousePosition.y < SplitterBottom && Event.current.mousePosition.y > SplitterTop)
                {
                    m_splitter = Event.current.mousePosition.y;
                }
                Repaint();
            }
        }
        if (m_splitter < SplitterTop) m_splitter = SplitterTop;
        if (m_splitter > SplitterBottom) m_splitter = SplitterBottom;

        Vector2 a = new Vector2(0, m_splitter);
        Vector2 b = new Vector2(position.width * 2, m_splitter);
        DrawLine(a, b, Color.black, 1);
    }

    private void GUICallStack()
    {
        int count = Debug.Container.GetLength();
        if (count == 0) return;

        m_messageGridPosition = GUILayout.BeginScrollView(m_messageGridPosition, GUILayout.Width(position.width), GUILayout.Height(m_splitter - 17));
        m_selectedMessage = SelectionList(m_selectedMessage, m_currentContent.ToArray(), m_styleTopFirst, m_styleTopSecond, OnMessageDoubleClick);
        GUILayout.EndScrollView();
        if (m_selectedMessage >= Debug.Container.GetLength()) return;

        GUILayout.Space(1);
        Log message = Debug.Container.GetLog(m_selectedMessage);
        List<GUIContent> callStack = new List<GUIContent>();
        foreach (var frame in message.Frames)
        {
            callStack.Add(new GUIContent(frame.ToString(), m_textureUp));
        }
        if (callStack.Count == 0) return;
        m_frameGridPosition = GUILayout.BeginScrollView(m_frameGridPosition, GUILayout.Width(position.width));
        m_selectedFrame = SelectionList(m_selectedFrame, callStack.ToArray(), m_styleBottomFirst, m_styleBottomSecond, OnFrameDoubleClick);
        GUILayout.EndScrollView();
    }

    #region Utils
    // copied form http://www.unifycommunity.com/wiki/index.php?title=DrawLine
    private void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, Texture2D lineTex = null)
    {
        Matrix4x4 matrix = GUI.matrix;
        if (!lineTex) { lineTex = new Texture2D(1, 1); }
        Color savedColor = GUI.color;
        GUI.color = color;
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
        if (pointA.y > pointB.y) { angle = -angle; }
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
        GUIUtility.RotateAroundPivot(angle, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

    // copied form http://www.unifycommunity.com/wiki/index.php?title=SelectionList
    private int SelectionList(int selected, GUIContent[] list, GUIStyle fStyle, GUIStyle sStyle, Action<int> callback)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            GUIStyle style = i % 2 == 0 ? fStyle : sStyle;
            Rect elementRect = GUILayoutUtility.GetRect(list[i], style);
            bool hover = elementRect.Contains(Event.current.mousePosition);
            if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
            {
                selected = i;
                Event.current.Use();
            }
            else if (hover && callback != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
            {
                callback(i);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.repaint)
            {
                style.Draw(elementRect, list[i], hover, false, i == selected, false);
            }
        }
        return selected;
    }
    #endregion

    private void Update()
    {
        //UnityDebug.Log(Time.time);
    }

    private float SplitterTop
    {
        get { return m_splitterOffset; }
    }

    private float SplitterBottom
    {
        get { return position.height - m_splitterOffset; }
    }

    private float m_splitter;
    private float m_splitterOffset = 25;
    private bool m_splitterDrag;

    private int m_selectedMessage = 0;
    private int m_selectedFrame = 0;
    private List<GUIContent> m_currentContent;

    private Vector2 m_messageGridPosition;
    private Vector2 m_frameGridPosition;

    private Texture2D m_textureBackground;
    private Texture2D m_textureError;
    private Texture2D m_textureLog;
    private Texture2D m_textureWarning;
    private Texture2D m_textureUp;

    private GUIStyle m_styleTopFirst;
    private GUIStyle m_styleTopSecond;
    private GUIStyle m_styleBottomFirst;
    private GUIStyle m_styleBottomSecond;
}