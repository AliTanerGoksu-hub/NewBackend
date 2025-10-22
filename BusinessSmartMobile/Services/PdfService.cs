using BusinessSmartMobile.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MudBlazor.CategoryTypes;

public class PdfService
{
    private static readonly SKColor PrimaryColor = SKColor.Parse("#3366CC");
    private static readonly SKColor SecondaryColor = SKColor.Parse("#F5F5F5");
    private static readonly SKColor BorderColor = SKColor.Parse("#CCCCCC");
    private static readonly SKColor TextGray = SKColor.Parse("#666666");
    private static readonly SKColor White = SKColors.White;
    private static readonly SKColor Black = SKColors.Black;
    private static string colorOpen = CommonParameters.colorOpen;
    private static string sizeOpen = CommonParameters.sizeOpen;
    private const float PageWidth = 842f;
    private const float PageHeight = 595f;
    private const float MarginLeft = 28.35f;
    private const float MarginRight = 28.35f;
    private const float MarginTop = 70f;
    private const float MarginBottom = 28.35f;
    private float currentY;
    private SKDocument document;
    private SKCanvas canvas;

    public byte[] CreateOrderPdf(List<UrunSecimi> order, TbFirma firma, TbParamGenel paramGenel, int siparisId, string personelAdi)
    {
        using var stream = new MemoryStream();
        document = SKDocument.CreatePdf(stream);
        canvas = document.BeginPage(PageWidth, PageHeight);
        currentY = MarginTop;
        AddHeader(paramGenel);
        AddCompanyAndOrderInfo(firma, siparisId);
        AddOrderDetailsTable(order);
        AddTotals(order);
        AddSignatureAreas(personelAdi);
        AddFooter(firma);
        document.EndPage();
        document.Close();
        return stream.ToArray();
    }

    private void AddHeader(TbParamGenel paramGenel)
    {
        using var titlePaint = new SKPaint { Color = PrimaryColor, TextSize = 13, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), IsAntialias = true };
        using var infoPaint = new SKPaint { Color = TextGray, TextSize = 9, Typeface = SKTypeface.FromFamilyName("Arial"), IsAntialias = true };
        using var companyPaint = new SKPaint { Color = PrimaryColor, TextSize = 11, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), IsAntialias = true };
        canvas.DrawText("Sipariş Belgesi", MarginLeft, currentY, titlePaint);
        string dateText = $"Tarih: {DateTime.Now:dd/MM/yyyy}";
        float dateWidth = infoPaint.MeasureText(dateText);
        canvas.DrawText(dateText, PageWidth - MarginRight - dateWidth, currentY, infoPaint);
        currentY += 25;
        canvas.DrawText(paramGenel.sFirmaAdi, MarginLeft, currentY, companyPaint);
        currentY += 20;
        canvas.DrawText($"Adres: {paramGenel.sFirmaAdresi}", MarginLeft, currentY, infoPaint);
        string phoneText = $"Telefon: {paramGenel.sTelefon1}";
        float phoneWidth = infoPaint.MeasureText(phoneText);
        canvas.DrawText(phoneText, PageWidth - MarginRight - phoneWidth, currentY, infoPaint);
        currentY += 30;
    }

    private void AddCompanyAndOrderInfo(TbFirma firma, int siparisId)
    {
        using var titlePaint = new SKPaint { Color = PrimaryColor, TextSize = 11, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), IsAntialias = true };
        using var infoPaint = new SKPaint { Color = TextGray, TextSize = 9, Typeface = SKTypeface.FromFamilyName("Arial"), IsAntialias = true };
        using var backgroundPaint = new SKPaint { Color = SecondaryColor, Style = SKPaintStyle.Fill };
        using var borderPaint = new SKPaint { Color = BorderColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };
        float contentWidth = PageWidth - MarginLeft - MarginRight;
        float halfWidth = contentWidth / 2;
        float cellHeight = 25;
        var headerRect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 10);
        canvas.DrawRect(headerRect, backgroundPaint);
        canvas.DrawRect(headerRect, borderPaint);
        canvas.DrawLine(MarginLeft + halfWidth, currentY - 15, MarginLeft + halfWidth, currentY + 10, borderPaint);
        canvas.DrawText("Müşteri Bilgileri", MarginLeft + 5, currentY, titlePaint);
        canvas.DrawText("Sipariş Bilgileri", MarginLeft + halfWidth + 5, currentY, titlePaint);
        currentY += cellHeight;
        var row1Rect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 10);
        canvas.DrawRect(row1Rect, borderPaint);
        canvas.DrawLine(MarginLeft + halfWidth, currentY - 15, MarginLeft + halfWidth, currentY + 10, borderPaint);
        canvas.DrawText($"Müşteri Adı: {firma.sAciklama}", MarginLeft + 5, currentY, infoPaint);
        canvas.DrawText($"Sipariş No: {siparisId}", MarginLeft + halfWidth + 5, currentY, infoPaint);
        currentY += cellHeight;
        var row2Rect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 10);
        canvas.DrawRect(row2Rect, borderPaint);
        canvas.DrawLine(MarginLeft + halfWidth, currentY - 15, MarginLeft + halfWidth, currentY + 10, borderPaint);
        canvas.DrawText($"Vergi Dairesi: {firma.sVergiDairesi}", MarginLeft + 5, currentY, infoPaint);
        canvas.DrawText($"Vergi No: {firma.sVergiNo}", MarginLeft + halfWidth + 5, currentY, infoPaint);
        currentY += 35;
    }

    private void AddOrderDetailsTable(List<UrunSecimi> order)
    {
        using var headerPaint = new SKPaint { Color = White, TextSize = 7, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), IsAntialias = true };
        using var cellPaint = new SKPaint { Color = Black, TextSize = 7, Typeface = SKTypeface.FromFamilyName("Arial"), IsAntialias = true };
        using var headerBackgroundPaint = new SKPaint { Color = PrimaryColor, Style = SKPaintStyle.Fill };
        using var altRowBackgroundPaint = new SKPaint { Color = SecondaryColor, Style = SKPaintStyle.Fill };
        using var borderPaint = new SKPaint { Color = BorderColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };

        bool hasDiscount = order.Any(x => x.Urun.nIskontoYuzdesi > 0);
        List<string> headers = new() { "ÜRÜN KODU", "BARKOD", "ÜRÜN AÇIKLAMASI", "SİPARİŞ", "KOLİ İÇİ", "LİSTE FİYAT" };
        List<float> columnWidths = new() { 0.14f, 0.10f, 0.30f, 0.07f, 0.07f, 0.10f };
        List<Func<UrunSecimi, string>> valueSelectors = new()
        {
            x => x.Urun.sKodu,
            x => x.Urun.sBarkod,
            x => x.Urun.sAciklama.Length > 53 ? x.Urun.sAciklama[..53] + "..." : x.Urun.sAciklama,
            x => x.Miktar.ToString(),
            x => x.Urun.nBirimCarpan.ToString(),
            x => $"₺{x.Urun.lFiyat1:N2}"
        };

        if (hasDiscount)
        {
            headers.AddRange(new[] { "İSKONTOLU\nFİYAT", "TOPLAM" });
            columnWidths.AddRange(new[] { 0.08f, 0.14f });
            valueSelectors.AddRange(new List<Func<UrunSecimi, string>>
            {
                x => x.Urun.nIskontoYuzdesi > 0 ? $"₺{(x.Urun.lFiyat1 - (x.Urun.lFiyat1 * x.Urun.nIskontoYuzdesi / 100)):N2}" : $"₺{x.Urun.lFiyat1:N2}",
                x => $"₺{x.Miktar * (x.Urun.nIskontoYuzdesi > 0 ? (x.Urun.lFiyat1 - (x.Urun.lFiyat1 * x.Urun.nIskontoYuzdesi / 100)) : x.Urun.lFiyat1):N2}"
            });
        }
        else
        {
            headers.Add("TOPLAM");
            columnWidths.Add(0.22f);
            valueSelectors.Add(x => $"₺{x.Miktar * x.Urun.lFiyat1:N2}");
        }

        float contentWidth = PageWidth - MarginLeft - MarginRight;
        float[] columnPositions = new float[headers.Count];
        columnPositions[0] = MarginLeft;
        for (int i = 1; i < headers.Count; i++)
            columnPositions[i] = columnPositions[i - 1] + (contentWidth * columnWidths[i - 1]);

        float rowHeight = 24;
        DrawTableHeader(headers, headerPaint, headerBackgroundPaint, borderPaint, columnPositions, rowHeight);
        currentY += rowHeight;

        bool alternate = false;
        foreach (var item in order)
        {
            if (currentY + rowHeight > PageHeight - MarginBottom)
            {
                document.EndPage();
                canvas = document.BeginPage(PageWidth, PageHeight);
                currentY = MarginTop;
                DrawTableHeader(headers, headerPaint, headerBackgroundPaint, borderPaint, columnPositions, rowHeight);
                currentY += rowHeight;
            }

            var rowRect = new SKRect(MarginLeft, currentY, PageWidth - MarginRight, currentY + rowHeight);
            if (alternate) canvas.DrawRect(rowRect, altRowBackgroundPaint);
            canvas.DrawRect(rowRect, borderPaint);
            for (int i = 1; i < headers.Count; i++)
                canvas.DrawLine(columnPositions[i], currentY, columnPositions[i], currentY + rowHeight, borderPaint);

            for (int i = 0; i < headers.Count; i++)
            {
                string text = valueSelectors[i](item).Replace("□", "");
                float columnWidth = (i < headers.Count - 1)
                    ? columnPositions[i + 1] - columnPositions[i]
                    : (PageWidth - MarginRight) - columnPositions[i];
                float textX = (i == headers.Count - 1)
                    ? columnPositions[i] + columnWidth - cellPaint.MeasureText(text) - 5
                    : columnPositions[i] + 3;
                float textY = currentY + rowHeight / 1.5f;
                canvas.DrawText(text, textX, textY, cellPaint);
            }
            currentY += rowHeight;
            alternate = !alternate;
        }
        currentY += 10;
    }

    private void DrawTableHeader(List<string> headers, SKPaint headerPaint, SKPaint backgroundPaint, SKPaint borderPaint, float[] columnPositions, float rowHeight)
    {
        var headerRect = new SKRect(MarginLeft, currentY, PageWidth - MarginRight, currentY + rowHeight);
        canvas.DrawRect(headerRect, backgroundPaint);
        canvas.DrawRect(headerRect, borderPaint);
        for (int i = 1; i < headers.Count; i++)
            canvas.DrawLine(columnPositions[i], currentY, columnPositions[i], currentY + rowHeight, borderPaint);
        for (int i = 0; i < headers.Count; i++)
        {
            string[] lines = headers[i].Split('\n');
            float columnWidth = (i < headers.Count - 1)
                ? columnPositions[i + 1] - columnPositions[i]
                : (PageWidth - MarginRight) - columnPositions[i];
            float textY = currentY + (lines.Length == 1 ? rowHeight / 1.5f : rowHeight / 2 - 2);
            foreach (string line in lines)
            {
                float textWidth = headerPaint.MeasureText(line);
                float textX = columnPositions[i] + (columnWidth - textWidth) / 2;
                canvas.DrawText(line, textX, textY, headerPaint);
                textY += 10;
            }
        }
    }

    private void AddTotals(List<UrunSecimi> order)
    {
        using var totalPaint = new SKPaint { Color = White, TextSize = 9, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), IsAntialias = true };
        using var totalBackgroundPaint = new SKPaint { Color = TextGray, Style = SKPaintStyle.Fill };
        using var grandTotalBackgroundPaint = new SKPaint { Color = PrimaryColor, Style = SKPaintStyle.Fill };
        using var borderPaint = new SKPaint { Color = BorderColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };

        bool hasDiscount = order.Any(x => x.Urun.nIskontoYuzdesi > 0);
        double toplamMiktar = order.Sum(i => i.Miktar);
        double toplamTutar = order.Sum(i => i.Miktar * i.Urun.lFiyat1);
        double toplamIskonto = hasDiscount ? order.Sum(i => i.Urun.lFiyat1 * i.Urun.nIskontoYuzdesi / 100 * i.Miktar) : 0;
        double genelToplam = toplamTutar - toplamIskonto;

        float contentWidth = PageWidth - MarginLeft - MarginRight;
        float labelWidth = contentWidth * 0.8f;
        if (currentY + (hasDiscount ? 80 : 60) > PageHeight - MarginBottom)
        {
            document.EndPage();
            canvas = document.BeginPage(PageWidth, PageHeight);
            currentY = MarginTop;
        }

        // Toplam Miktar
        var totalQuantityRect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 5);
        canvas.DrawRect(totalQuantityRect, totalBackgroundPaint);
        canvas.DrawRect(totalQuantityRect, borderPaint);
        canvas.DrawLine(MarginLeft + labelWidth, currentY - 15, MarginLeft + labelWidth, currentY + 5, borderPaint);
        canvas.DrawText("Toplam Miktar:", MarginLeft + 5, currentY, totalPaint);
        string toplamMiktarText = $"{toplamMiktar}";
        float miktarWidth = totalPaint.MeasureText(toplamMiktarText);
        canvas.DrawText(toplamMiktarText, PageWidth - MarginRight - miktarWidth - 5, currentY, totalPaint);
        currentY += 20;

        // Toplam (İskontosuz)
        var totalWithoutDiscountRect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 5);
        canvas.DrawRect(totalWithoutDiscountRect, totalBackgroundPaint);
        canvas.DrawRect(totalWithoutDiscountRect, borderPaint);
        canvas.DrawLine(MarginLeft + labelWidth, currentY - 15, MarginLeft + labelWidth, currentY + 5, borderPaint);
        canvas.DrawText("Toplam:", MarginLeft + 5, currentY, totalPaint);
        string toplamTutarText = $"₺{toplamTutar:N2}";
        float tutarWidth = totalPaint.MeasureText(toplamTutarText);
        canvas.DrawText(toplamTutarText, PageWidth - MarginRight - tutarWidth - 5, currentY, totalPaint);
        currentY += 20;

        // Toplam İskonto (sadece iskonto varsa)
        if (hasDiscount)
        {
            var discountRect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 5);
            canvas.DrawRect(discountRect, totalBackgroundPaint);
            canvas.DrawRect(discountRect, borderPaint);
            canvas.DrawLine(MarginLeft + labelWidth, currentY - 15, MarginLeft + labelWidth, currentY + 5, borderPaint);
            canvas.DrawText("Toplam İskonto:", MarginLeft + 5, currentY, totalPaint);
            string toplamIskontoText = $"₺{toplamIskonto:N2}";
            float iskontoWidth = totalPaint.MeasureText(toplamIskontoText);
            canvas.DrawText(toplamIskontoText, PageWidth - MarginRight - iskontoWidth - 5, currentY, totalPaint);
            currentY += 20;
        }

        // Genel Toplam
        var grandTotalRect = new SKRect(MarginLeft, currentY - 15, PageWidth - MarginRight, currentY + 5);
        canvas.DrawRect(grandTotalRect, grandTotalBackgroundPaint);
        canvas.DrawRect(grandTotalRect, borderPaint);
        canvas.DrawLine(MarginLeft + labelWidth, currentY - 15, MarginLeft + labelWidth, currentY + 5, borderPaint);
        canvas.DrawText("Genel Toplam:", MarginLeft + 5, currentY, totalPaint);
        string genelToplamText = $"₺{genelToplam:N2}";
        float genelToplamWidth = totalPaint.MeasureText(genelToplamText);
        canvas.DrawText(genelToplamText, PageWidth - MarginRight - genelToplamWidth - 5, currentY, totalPaint);
        currentY += 35;
    }

    private void AddSignatureAreas(string personelAdi)
    {
        using var signaturePaint = new SKPaint { Color = TextGray, TextSize = 9, Typeface = SKTypeface.FromFamilyName("Arial"), IsAntialias = true };
        using var linePaint = new SKPaint { Color = TextGray, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };
        canvas.DrawText("Siparişi Alan", MarginLeft, currentY, signaturePaint);
        currentY += 20;
        canvas.DrawText($"Ad Soyad: {personelAdi}", MarginLeft, currentY, signaturePaint);
        currentY += 20;
        canvas.DrawText("İmza:", MarginLeft, currentY, signaturePaint);
        canvas.DrawLine(MarginLeft + 40, currentY + 2, MarginLeft + 230, currentY + 2, linePaint);
        currentY += 40;
    }

    private void AddFooter(TbFirma firma)
    {
        using var footerPaint = new SKPaint { Color = TextGray, TextSize = 7, Typeface = SKTypeface.FromFamilyName("Arial"), IsAntialias = true };
        string belgeNo = $"Belge No: SIP-{firma.nFirmaId}-{DateTime.Now:yyyyMMdd}";
        string dijitalText = "Bu belge dijital olarak oluşturulmuştur.";
        string barkodText = "©Barkod Yazılım. www.barkodyazilim.com.tr info@bakodyazilim.com.tr";
        float belgeNoWidth = footerPaint.MeasureText(belgeNo);
        float dijitalWidth = footerPaint.MeasureText(dijitalText);
        float barkodWidth = footerPaint.MeasureText(barkodText);
        float contentWidth = PageWidth - MarginLeft - MarginRight;
        canvas.DrawText(belgeNo, MarginLeft + (contentWidth - belgeNoWidth) / 2, currentY, footerPaint);
        currentY += 15;
        canvas.DrawText(dijitalText, MarginLeft + (contentWidth - dijitalWidth) / 2, currentY, footerPaint);
        currentY += 15;
        canvas.DrawText(barkodText, MarginLeft + (contentWidth - barkodWidth) / 2, currentY, footerPaint);
    }
}