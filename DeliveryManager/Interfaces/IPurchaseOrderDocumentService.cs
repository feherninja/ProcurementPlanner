using DeliveryManager.Services;

namespace DeliveryManager.Interfaces;

public interface IPurchaseOrderDocumentService
{
	byte[] GeneratePurchaseOrderDocument(DeliveryPlanningService planningService);
}