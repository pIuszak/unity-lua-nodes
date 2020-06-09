using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class UINodeLineRenderer : UILineRenderer
{
    private bool updateLine;
    private readonly int x = Screen.width / 2;
    private readonly int y = Screen.height / 2;
    
    [UsedImplicitly]
    public void StartLine()
    {
        updateLine = true;
        var position = transform.position;
        m_points[0] = new Vector2(position.x, position.y);
    }
    
    [UsedImplicitly]
    public void EndLine()
    {
        updateLine = false;
    }

    // talk dirty to update line renderer and set mose pos
    // todo: this is for demo only, to heavy for production code 
    private void Update()
    {
        if (!updateLine) return;
        SetAllDirty();
        var position = Input.mousePosition;
        m_points[1] = new Vector2(position.x - x, position.y - y);
    }
}