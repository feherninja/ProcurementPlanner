using DeliveryManager.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDFContainer = QuestPDF.Infrastructure.IContainer;

namespace DeliveryManager.Services;

public class PurchaseOrderDocumentService : IPurchaseOrderDocumentService
{
    public byte[] GeneratePurchaseOrderDocument(DeliveryPlanningService planningService)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header().Element(ComposeHeader); // keep as-is
                page.Content().Element(content => ComposeContent(content, planningService));
                page.Footer().Element(ComposeFooter); // keep as-is
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(QuestPDFContainer container)
    {
        // unchanged from your DeliveryDocumentService
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().PaddingBottom(5).Text("PIRÓTH & TÁRSA IPARI ÉS KERESKEDELMI BETÉTI TÁRSASÁG")
                       .AlignCenter()
                       .FontSize(11)
                       .Bold()
                       .Underline();

                    col.Item().Text("2310 Szigetszentmiklós Móra Ferenc utca 16")
                       .AlignCenter()
                       .FontSize(9);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight().PaddingRight(5).Text($"email: pirothbt@pirothbt.hu")
                           .Underline()
                           .FontSize(9);
                        row.RelativeItem().Text($"Telefon: + 36 70 977 3225")
                           .FontSize(9);
                    });
                });
            });

            column.Item().PaddingTop(15);
        });
    }

    private void ComposeContent(QuestPDFContainer container, DeliveryPlanningService planningService)
    {
        container.Column(column =>
        {
            // keep same "recipient" block style as your current doc
            var customer = planningService.SelectedCompany;
            if (customer != null)
            {
                column.Item().Text(customer.Name).FontSize(11).Bold();

                if (!string.IsNullOrWhiteSpace(customer.Address))
                    column.Item().Text(customer.Address).FontSize(10);

                if (!string.IsNullOrWhiteSpace(customer.Email))
                    column.Item().Text(customer.Email).FontSize(9).Underline();
            }

            column.Item().PaddingVertical(15).Row(row =>
            {
                row.RelativeItem().Text($"Objekt: Bestellung").FontSize(10);
                row.RelativeItem().AlignRight().Text($"{planningService.DateOfDelivery:yyyy-MM-dd}").FontSize(10);
            });

            column.Item().Text($"Bestellnummer: {planningService.DeliveryNumber}")
                  .FontSize(10);

            column.Item().PaddingTop(20).Element(content => ComposeTable(content, planningService));
        });
    }

    private void ComposeTable(QuestPDFContainer container, DeliveryPlanningService planningService)
    {
        var shortName = planningService.SelectedCompany?.ShortName;
        var artNoHeader = string.IsNullOrWhiteSpace(shortName) ? "Art.no." : $"{shortName} {Environment.NewLine} Art.no.";

        var componentsToOrder = planningService.RequiredComponents
            .Where(rc => rc.CalculateRequiredOrderQuantity() > 0)
            .OrderBy(rc => rc.Component.Name)
            .ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4);    // Komponentenbezeichnung
                columns.RelativeColumn(2);    // {Company.ShortName} Art.no.
                columns.RelativeColumn(2);    // Zeichnungsnummer
                columns.RelativeColumn(1.5f); // Bestellte Menge (packages)
                columns.RelativeColumn(1);    // ME
                columns.RelativeColumn(1.5f); // Lieferzeit
            });

            table.Header(header =>
            {
                header.Cell().Border(1).Padding(3).Text("Komponentenbezeichnung")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text(artNoHeader)
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Zeichnungsnummer")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Bestellte Menge")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("ME")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Lieferzeit")
                      .FontSize(9).Bold().AlignCenter();
            });

            foreach (var rc in componentsToOrder)
            {
                var c = rc.Component;

                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.Name).FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.PartNumber ?? "").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.DrawingNumber ?? "").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(rc.CalculateRequiredOrderQuantity().ToString()).FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text("St.").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text($"Woche {planningService.WeekOfDelivery}").FontSize(8);
            }
        });
    }

    private void ComposeFooter(QuestPDFContainer container)
    {
        // unchanged from your DeliveryDocumentService
        container.AlignBottom().Column(column =>
        {
            column.Item().PaddingTop(15).Text("Mit freundlichen Grüßen:")
                  .FontSize(10);

            column.Item().PaddingTop(10).Text("Schultz Gábor")
                  .FontSize(10);

            column.Item().PaddingTop(10).Text("Lieferadresse: Ungarn 2310 Szigetszentmiklós Határ Út 12-14.")
                  .FontSize(10).Bold();
        });
    }
}