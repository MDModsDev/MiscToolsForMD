namespace MiscToolsForMD.Sdk;

/// <summary>
/// Standard attribute with some extends.
/// </summary>
public abstract class ExtendedAttribute : Attribute
{
    /// <summary>
    /// If this attribute is checked.
    /// </summary>
    public bool isChecked = false;
}
