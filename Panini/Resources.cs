using System;
using System.Reflection;
using UnityEngine;

namespace PaniniPlugin.Panini;

public static class Resources
{
    public static readonly Lazy<AssetBundle> Assets = new(LoadBundle);

    private static AssetBundle LoadBundle()
    {
        var info = Assembly.GetExecutingAssembly().GetName();
        var name = info.Name;
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{name}.Assets.bundle")!;

        if (stream == null) throw new Exception("Could not open asset bundle!");

        return AssetBundle.LoadFromStream(stream);
    }
}