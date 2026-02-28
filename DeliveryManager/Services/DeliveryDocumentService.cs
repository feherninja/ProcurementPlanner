using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDFContainer = QuestPDF.Infrastructure.IContainer; // Create alias

namespace DeliveryManager.Services
{
	public interface IDeliveryDocumentService
	{
		byte[] GenerateDeliveryDocument(DeliveryPlanningService planningService);
	}

	public class DeliveryDocumentService : IDeliveryDocumentService
	{
		public byte[] GenerateDeliveryDocument(DeliveryPlanningService planningService)
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
					page.Footer().Element(ComposeFooter);
				});
			});

			return document.GeneratePdf();
		}

		private void ComposeHeader(QuestPDFContainer container)
		{
			container.Column(column =>
			{
				// Top row with company name and date
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
				// Recipient / Customer info (dynamic)
				var customer = planningService.SelectedCompany;

				var customerName = customer?.Name       ?? string.Empty;
				var customerAddress = customer?.Address ?? string.Empty;
				var customerEmail = customer?.Email     ?? string.Empty;

				if (!string.IsNullOrWhiteSpace(customerName))
				{
					column.Item().Text(customerName)
					      .FontSize(11)
					      .Bold();
				}

				if (!string.IsNullOrWhiteSpace(customerAddress))
				{
					column.Item().Text(customerAddress)
					      .FontSize(10);
				}

				if (!string.IsNullOrWhiteSpace(customerEmail))
				{
					column.Item().Text(customerEmail)
					      .FontSize(9)
					      .Underline();
				}

				// Meta row: object + date
				column.Item().PaddingVertical(15).Row(row =>
				{
					row.RelativeItem().Text($"Objekt: Bestellung")
					   .FontSize(10);

					row.RelativeItem().AlignRight().Text($"{planningService.DateOfDelivery:yyyy-MM-dd}")
					   .FontSize(10);
				});

				// Delivery number
				column.Item().Text($"Bestellnummer: {planningService.DeliveryNumber}")
				      .FontSize(10);

				// Table
				column.Item().PaddingTop(20).Element(content => ComposeTable(content, planningService));
			});
		}

		private void ComposeTable(QuestPDFContainer container, DeliveryPlanningService planningService)
		{
			var customer = planningService.SelectedCompany;
			var customerShortName = customer?.ShortName ?? string.Empty;

			container.Table(table =>
			{
				// Define columns
				table.ColumnsDefinition(columns =>
				{
					columns.RelativeColumn(4);    // Komponentenbezeichnung
					columns.RelativeColumn(2);    // Aspöck Art.no.
					columns.RelativeColumn(2);    // Komponente Schwarzmüller  
					columns.RelativeColumn(1.5f); // Bestellte Menge
					columns.RelativeColumn(1);    // ME
					columns.RelativeColumn(1.5f); // Lieferzeit
				});

				// Header row
				table.Header(header =>
				{
					header.Cell().Border(1).Padding(3).Text("Komponentenbezeichnung")
					      .FontSize(9).Bold().AlignCenter();

					header.Cell().Border(1).Padding(3).Text($"{customerShortName} {Environment.NewLine} Art.no.")
					      .FontSize(9).Bold().AlignCenter();

					header.Cell().Border(1).Padding(3).Text("Komponente Schwarzmüller")
					      .FontSize(9).Bold().AlignCenter();

					header.Cell().Border(1).Padding(3).Text("Bestellte Menge")
					      .FontSize(9).Bold().AlignCenter();

					header.Cell().Border(1).Padding(3).Text("ME")
					      .FontSize(9).Bold().AlignCenter();

					header.Cell().Border(1).Padding(3).Text("Lieferzeit")
					      .FontSize(9).Bold().AlignCenter();
				});

				// Data rows
				foreach (var selectedItem in planningService.SelectedItems)
				{
					table.Cell().Border(1).Padding(3).AlignCenter().Text(selectedItem.Item.Name).FontSize(8);
					table.Cell().Border(1).Padding(3).AlignCenter().Text(selectedItem.Item.PartNumber    ?? "").FontSize(8);
					table.Cell().Border(1).Padding(3).AlignCenter().Text(selectedItem.Item.DrawingNumber ?? "").FontSize(8);
					table.Cell().Border(1).Padding(3).AlignCenter().Text(selectedItem.Quantity.ToString()).FontSize(8);
					table.Cell().Border(1).Padding(3).AlignCenter().Text("St.").FontSize(8);
					table.Cell().Border(1).Padding(3).AlignCenter().Text($"Woche {planningService.WeekOfDelivery}").FontSize(8);
				}
			});
		}

		private void ComposeFooter(QuestPDFContainer container)
		{
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
}