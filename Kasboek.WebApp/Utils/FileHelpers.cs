﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kasboek.WebApp.Utils
{
    /// <summary>
    /// Based on https://docs.microsoft.com/en-us/aspnet/core/razor-pages/upload-files
    /// </summary>
    public class FileHelpers
    {
        public static async Task<string> ProcessFormFile(IFormFile formFile, ModelStateDictionary modelState)
        {
            var csvContentTypes = new List<string> { "text/plain", "text/csv", "text/x-csv", "application/vnd.ms-excel" };

            // Use Path.GetFileName to obtain the file name, which will
            // strip any path information passed as part of the
            // FileName property. HtmlEncode the result in case it must 
            // be returned in an error message.
            var fileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));

            if (!csvContentTypes.Contains(formFile.ContentType.ToLower()))
            {
                modelState.AddModelError(formFile.Name, $"The file ({fileName}) must be a text file.");
                return string.Empty;
            }

            // Check the file length and don't bother attempting to
            // read it if the file contains no content. This check
            // doesn't catch files that only have a BOM as their
            // content, so a content length check is made later after 
            // reading the file's content to catch a file that only
            // contains a BOM.
            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.Name, $"The file ({fileName}) is empty.");
            }
            else if (formFile.Length > 1048576)
            {
                modelState.AddModelError(formFile.Name, $"The file ({fileName}) exceeds 1 MiB.");
            }
            else
            {
                try
                {
                    string fileContents;

                    // The StreamReader is created to read files that are UTF-8 encoded. 
                    // If uploads require some other encoding, provide the encoding in the 
                    // using statement. To change to 32-bit encoding, change 
                    // new UTF8Encoding(...) to new UTF32Encoding().
                    using (var reader = new StreamReader(
                        formFile.OpenReadStream(),
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
                        detectEncodingFromByteOrderMarks: true))
                    {
                        fileContents = await reader.ReadToEndAsync();

                        // Check the content length in case the file's only
                        // content was a BOM and the content is actually
                        // empty after removing the BOM.
                        if (fileContents.Length > 0)
                        {
                            return fileContents;
                        }
                        else
                        {
                            modelState.AddModelError(formFile.Name, $"The file ({fileName}) is empty.");
                        }
                    }
                }
                catch (Exception)
                {
                    modelState.AddModelError(formFile.Name, $"The file ({fileName}) upload failed.");
                }
            }

            return string.Empty;
        }
    }
}