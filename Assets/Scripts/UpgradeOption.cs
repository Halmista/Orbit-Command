using System;

public class UpgradeOption
{
    public string name;
    public Action action;

    public UpgradeOption(string name, Action action)
    {
        this.name = name;
        this.action = action;
    }
}