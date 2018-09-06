CREATE OR ALTER VIEW report.PurchaseOrderBalanceView AS
SELECT
	CAST(row_number() OVER (ORDER BY po.Number) AS INT) AS Id,
	po.Number,
	po.ClientExternalId,
	po.ClientExternalName,
	po.FileId,
	po.Proposal,
	fil.FileName,
	po.Id as PurchaseOrderId,
	poad.CurrencyId,
	cur.Text as CurrencyText,
	poad.Ammount,
	poad.Adjustment,
	po.Status,
	po.ReceptionDate,
	IIF(poad.AdjustmentBalance IS NULL, 
		poad.Ammount - SUM(IIF(hit.Total IS NULL, 0, hit.Total))
		, poad.AdjustmentBalance)
	 as Balance,
	STRING_AGG(an.Id, ';') as AnalyticIds,
	STRING_AGG(an.ManagerId, ';') as ManagerIds,
	STRING_AGG(an.CommercialManagerId, ';') as CommercialManagerIds,
	STRING_AGG(accu.Name, ';') as AccountManagerNames,
	STRING_AGG(proju.Name, ';') as ProjectManagerNames
FROM app.PurchaseOrders po
LEFT JOIN app.PurchaseOrderAmmountDetails poad ON poad.PurchaseOrderId = po.Id
LEFT JOIN app.PurchaseOrderAnalytics poan ON poan.PurchaseOrderId = po.id
LEFT JOIN app.Analytics an ON an.Id = poan.AnalyticId
LEFT JOIN app.Solfacs sf ON 
		sf.PurchaseOrderId = poad.PurchaseOrderId 
		AND sf.CurrencyId = poad.CurrencyId
		AND sf.ServiceId = an.ServiceId
LEFT JOIN app.Hitos hit ON hit.SolfacId = sf.Id
LEFT JOIN app.Currencies cur ON cur.Id = poad.CurrencyId
LEFT JOIN app.[Users] accu ON accu.Id = an.ManagerId
LEFT JOIN app.[Users] proju ON proju.Id = an.CommercialManagerId
LEFT JOIN app.Files fil ON fil.Id = po.FileId
GROUP BY 
	po.Number, po.ClientExternalId, po.Id, po.ClientExternalName, po.FileId, 
	fil.FileName, poad.CurrencyId, cur.Text, Ammount, po.ReceptionDate, 
	po.Status, poad.Adjustment, poad.AdjustmentBalance, po.Proposal
