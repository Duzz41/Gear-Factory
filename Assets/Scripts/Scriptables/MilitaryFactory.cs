using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryFactory : Building
{
    private int tool_count;
    [SerializeField] private int max_tool_count;
    [SerializeField] private GameObject my_tool;
    [SerializeField] private Transform tool_stand;
    public override void Build()
    {
        base.Build();
    }

    public override void Upgrade()
    {
        base.Upgrade();

        if (tool_count < max_tool_count)
        {
            lock_my_UI = false;
            Instantiate(my_tool, tool_stand.position + new Vector3(tool_stand.childCount * 1, 0, 0), Quaternion.identity, tool_stand);
            tool_count += 1;
        }
        if (tool_count == max_tool_count)
        {
            lock_my_UI = true;
            my_canvas.gameObject.SetActive(false);
        }

    }

    public override void BuyFrombuilding()
    {
        base.BuyFrombuilding();
    }
}
