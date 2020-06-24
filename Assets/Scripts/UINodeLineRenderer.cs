using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class UINodeLineRenderer : UILineRenderer
{
    private bool updateLine;
    private readonly int x = Screen.width / 2;
    private readonly int y = Screen.height / 2;

    //public GameObject thing;
    public Canvas myCanvas;
    public GameObject src;
    public GameObject dest;


    [UsedImplicitly]
    public void StartLine()
    {
        updateLine = true;
        var position = transform.position;
        //m_points[0] = new Vector2(position.x, position.y);
    }

    void Start()
    {
        SetAllDirty();
        myCanvas = GetComponentInParent<Canvas>();
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
        if (dest != null) UpdateToOther();
      //  if (updateLine) UpdateToMouse();


    }

    private void UpdateToMouse()
    {
        SetAllDirty();
        //var position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform,
            Input.mousePosition, myCanvas.worldCamera, out pos);
        m_points[1] = pos;
    }
    
    private void UpdateToOther()
    {
        SetAllDirty();
        //var position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        // Vector2 pos;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform,
        
        
        //     other.transform.position, myCanvas.worldCamera, out pos);
        
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform,
            new Vector2(src.transform.position.x,src.transform.position.y), myCanvas.worldCamera, out pos);
        m_points[0] = src.transform.localPosition;
        
        m_points[1] = dest.transform.localPosition;
    }    
}