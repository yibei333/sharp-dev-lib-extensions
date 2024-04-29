namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// datatable dynmic key abstraction
/// </summary>
public interface IDynamicKey
{
    ///// <summary>
    ///// datatable dynmic key,support types:Guid,String,NumberTypes,Enum
    ///// </summary>
    //object DynamicKey { get; }

    /// <summary>
    /// dynamic name
    /// </summary>
    string DynamicName { get; set; }
}
