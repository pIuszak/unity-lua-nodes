using System;
using UnityEditor;
using UnityEngine;
 
public class Connection
{
    public ConnectionData connectionData;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public Action<Connection> OnClickRemoveConnection;
 
    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;

        connectionData = new ConnectionData();
        connectionData.inPointID = inPoint.node != null ? inPoint.node.nodeData.id : 0;
        connectionData.outPointID = outPoint.node != null ? outPoint.node.nodeData.id : 0;

        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }
 
    public void Draw()
    {
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );
 
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}