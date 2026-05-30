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

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, planningService));
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
                    col.Item().PaddingBottom(5).Text("PIRÓTH ÉS TÁRSA IPARI ÉS KERESKEDELMI BETÉTI TÁRSASÁG")
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

            column.Item().PaddingTop(15).Text("Mit freundlichen Grüßen:").FontSize(10);
            column.Item().PaddingTop(10).Text("Schultz Gábor").FontSize(10);
            column.Item().PaddingTop(10).Text("Lieferadresse: Ungarn 2310 Szigetszentmiklós Határ út 12-14.").FontSize(10).Bold();
        });
    }

    private void ComposeTable(QuestPDFContainer container, DeliveryPlanningService planningService)
    {
        var componentsToOrder = planningService.RequiredComponents
            .Where(rc => rc.CalculateRequiredOrderQuantity() > 0)
            .OrderBy(rc => rc.Component.Name)
            .ToList();

        var lieferdatum = planningService.DateOfDelivery?.ToString("dd.MM.yyyy") ?? "";

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(20);   // Nr.
                columns.RelativeColumn(4);    // Bezeichnung
                columns.RelativeColumn(2);    // Artikel-Nr.
                columns.RelativeColumn(2);    // Schwarzmüller-Nr.
                columns.RelativeColumn(1.5f); // Menge
                columns.RelativeColumn(1);    // Einheit
                columns.RelativeColumn(1.5f); // Lieferdatum
            });

            table.Header(header =>
            {
                header.Cell().Border(1).Padding(3).Text("Nr.")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Bezeichnung")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Materialnummer 1")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Materialnummer 2")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Menge")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Einheit")
                      .FontSize(9).Bold().AlignCenter();

                header.Cell().Border(1).Padding(3).Text("Lieferdatum")
                      .FontSize(9).Bold().AlignCenter();
            });

            int rowNumber = 1;
            foreach (var rc in componentsToOrder)
            {
                var c = rc.Component;

                table.Cell().Border(1).Padding(3).AlignCenter().Text(rowNumber++.ToString()).FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.Name).FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.PartNumber ?? "").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(c.ExternalNumber ?? "").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text((rc.CalculateRequiredOrderQuantity() * rc.Component.OrderPackageSize).ToString()).FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text("St.").FontSize(8);
                table.Cell().Border(1).Padding(3).AlignCenter().Text(lieferdatum).FontSize(8);
            }
        });
    }

}