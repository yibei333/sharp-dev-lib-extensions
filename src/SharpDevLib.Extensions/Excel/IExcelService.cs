using System.Data;

namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// excel read/write service
/// </summary>
public interface IExcelService
{
    #region Encrypt/Decrypt
    /// <summary>
    /// encrypt excel file
    /// </summary>
    /// <param name="filePath">excel file path</param>
    /// <param name="password">password</param>
    /// <param name="encryptedFilePath">save path</param>
    void Encrypt(string filePath, string password, string encryptedFilePath);
    /// <summary>
    /// decrypt excel file
    /// </summary>
    /// <param name="filePath">excel file path</param>
    /// <param name="password">password</param>
    /// <param name="decryptedFilePath">save path</param>
    void Decrypt(string filePath, string password, string decryptedFilePath);
    /// <summary>
    /// encrypt excel stream
    /// </summary>
    /// <param name="stream">origin stream</param>
    /// <param name="password">password</param>
    /// <returns>encrypted stream</returns>
    MemoryStream Encrypt(Stream stream, string password);
    /// <summary>
    /// decrypt excel stream
    /// </summary>
    /// <param name="stream">encrypted stream</param>
    /// <param name="password">password</param>
    /// <returns>origin stream</returns>
    MemoryStream Decrypt(Stream stream, string password);
    #endregion

    #region Read
    /// <summary>
    /// read excel file to datatable
    /// </summary>
    /// <param name="filePath">file path</param>
    /// <param name="options">read options</param>
    /// <returns>datatable</returns>
    DataTable[] Read(string filePath, params ExcelReadOptions[] options);
    /// <summary>
    /// read excel stream to datatable
    /// </summary>
    /// <param name="stream">excel stream</param>
    /// <param name="options">read options</param>
    /// <returns>datatable</returns>
    DataTable[] Read(Stream stream, params ExcelReadOptions[] options);
    /// <summary>
    /// read excel file to datatable
    /// </summary>
    /// <param name="filePath">file path</param>
    /// <param name="password">password</param>
    /// <param name="options">read options</param>
    /// <returns>datatable</returns>
    DataTable[] Read(string filePath, string password, params ExcelReadOptions[] options);
    /// <summary>
    /// read excel stream to datatable
    /// </summary>
    /// <param name="stream">excel stream</param>
    /// <param name="password">password</param>
    /// <param name="options">read options</param>
    /// <returns>datatable</returns>
    DataTable[] Read(Stream stream, string password, params ExcelReadOptions[] options);
    #endregion

    #region Write
    /// <summary>
    /// write excel to bytes
    /// </summary>
    /// <param name="options">write options</param>
    /// <returns>excel bytes</returns>
    byte[] Write(params ExcelWriteOptions[] options);

    /// <summary>
    /// write excel to bytes
    /// </summary>
    /// <param name="password">password</param>
    /// <param name="options">write options</param>
    /// <returns>excel bytes</returns>
    byte[] Write(string password, params ExcelWriteOptions[] options);
    #endregion
}
