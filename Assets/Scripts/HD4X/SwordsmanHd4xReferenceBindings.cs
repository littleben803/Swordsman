using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Runs before ordinary scene components; prefab loading also binds before callers inspect it.
[DefaultExecutionOrder(-32000)]
public sealed class SwordsmanHd4xReferenceBindings : MonoBehaviour
{
    [Serializable]
    public sealed class Binding
    {
        public Component target;
        public string propertyPath;
        public string referenceId;
    }
    public Binding[] bindings = new Binding[0];

    void Awake() { Apply(); }

    public void Apply()
    {
        foreach (Binding binding in bindings)
        {
            if (binding.target == null) throw new InvalidOperationException("HD_REQUIRED: Missing binding target.");
            Sprite sprite = SwordsmanHd4xSpriteResolver.ResolveReference(binding.referenceId);
            SetSprite(binding.target, binding.propertyPath, sprite);
        }
    }

    public static void ApplyAll(GameObject root)
    {
        foreach (var item in root.GetComponentsInChildren<SwordsmanHd4xReferenceBindings>(true)) item.Apply();
    }

    public static void SetSprite(Component target, string path, Sprite sprite)
    {
        var image = target as Image;
        if (image != null && path == "m_Sprite") { image.sprite = sprite; return; }
        var renderer = target as SpriteRenderer;
        if (renderer != null && path == "m_Sprite") { renderer.sprite = sprite; return; }
        SetField(target, path.Replace(".Array.data[", "[").Split('.'), 0, sprite);
    }

    static void SetField(object target, string[] parts, int position, Sprite sprite)
    {
        string part = parts[position];
        int bracket = part.IndexOf('[');
        string name = bracket < 0 ? part : part.Substring(0, bracket);
        FieldInfo field = null;
        for (Type type = target.GetType(); type != null && field == null; type = type.BaseType)
            field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (field == null) throw new InvalidOperationException("HD_REQUIRED: Unsupported binding field: " + name);
        object value = field.GetValue(target);
        if (bracket >= 0)
        {
            int index = int.Parse(part.Substring(bracket + 1, part.Length - bracket - 2));
            Array array = (Array)value;
            if (position == parts.Length - 1) array.SetValue(sprite, index);
            else { object element = array.GetValue(index); SetField(element, parts, position + 1, sprite); array.SetValue(element, index); }
        }
        else if (position == parts.Length - 1) value = sprite;
        else SetField(value, parts, position + 1, sprite);
        field.SetValue(target, value);
    }
}
