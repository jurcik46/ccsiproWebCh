﻿namespace WebChromiumCcsipro.Resources.Interfaces
{
    public interface ISignatureFileModel
    {
        string PdfFilePath { get; set; }
        string Hash { get; set; }
        byte[] File { get; set; }

    }
}