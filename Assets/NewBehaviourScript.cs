using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    static Table Sum(Table t)
    {
        var nums = from v in t.Values
            where v.Type == DataType.Number
            select v.Number;

        return t;
    }


    private Table TableTestReverseWithTable(Table t)
    {
        string scriptCode = @"    
        return dosum { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ,123,123,12,31,23}";

        Script script = new Script();

        script.Globals["dosum"] = (Func<Table, Table>) Sum;

        DynValue res = script.DoString(scriptCode);

        return res.Table;
    }

    [Button]
    public void Test()
    {
        // Debug.Log(TableTestReverseWithTable(new Table(0,0)));
    }
}