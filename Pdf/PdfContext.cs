﻿namespace PdfMerger.Pdf;

internal class PdfContext
{
    public PdfReference Root { get; set; }
    public PdfScope Scope { get; set; }
    public List<PdfReferenceValue> Pages { get; } = new(10);
    public List<PdfReference> References { get; } = new(100);
}