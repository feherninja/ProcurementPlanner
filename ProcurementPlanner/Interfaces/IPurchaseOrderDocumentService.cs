using ProcurementPlanner.Services;

namespace ProcurementPlanner.Interfaces;

public interface IPurchaseOrderDocumentService
{
	byte[] GeneratePurchaseOrderDocument(DeliveryPlanningService planningService);
}