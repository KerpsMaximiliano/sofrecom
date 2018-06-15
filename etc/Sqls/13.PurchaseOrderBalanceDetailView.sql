CREATE OR ALTER VIEW report.PurchaseOrderBalanceDetailView AS
SELECT
		hit.Id,
		sf.Analytic,
		an.Name,
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
LEFT JOIN app.Analytics an ON an.Title = sf.Analytic AND an.ServiceId = sf.ServiceId