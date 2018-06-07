CREATE OR ALTER VIEW app.PurchaseOrderBalanceView AS
SELECT 
	CAST(row_number() OVER (ORDER BY po.Number) AS INT) AS Id,
	po.Number,
	po.ClientExternalId,
	po.ClientExternalName,
	po.Id as PurchaseOrderId,
	poad.CurrencyId,
	cur.Text as CurrencyText,
	poad.Ammount,
	po.Status,
	SUM(hit.Total) as Balance
FROM app.PurchaseOrders po
INNER JOIN app.PurchaseOrderAmmountDetails poad ON poad.PurchaseOrderId = po.Id
INNER JOIN app.Solfacs sf ON sf.PurchaseOrderId = poad.PurchaseOrderId AND sf.CurrencyId = poad.CurrencyId
INNER JOIN app.Hitos hit ON hit.SolfacId = sf.Id
LEFT JOIN app.Currencies cur ON cur.Id = poad.CurrencyId
GROUP BY 
	po.Number, po.ClientExternalId, po.Id, po.ClientExternalName, poad.CurrencyId, cur.Text, Ammount, po.Status

CREATE OR ALTER VIEW app.PurchaseOrderBalanceDetailView AS
SELECT
hit.Id,
sf.Id as SolfacId,
hit.Description,
sf.UpdatedDate,
hit.Total,
sf.CurrencyId, 
cur.Text as CurrencyText,
sf.Status,
sf.PurchaseOrderId
FROM app.Hitos hit
INNER JOIN app.Solfacs sf ON sf.Id = hit.SolfacId
LEFT JOIN app.Currencies cur ON cur.Id = sf.CurrencyId