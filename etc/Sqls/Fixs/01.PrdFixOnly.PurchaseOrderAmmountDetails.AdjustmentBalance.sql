IF COL_LENGTH('app.PurchaseOrderAmmountDetails', 'AdjustmentBalance') IS NULL
BEGIN
	ALTER TABLE app.[PurchaseOrderAmmountDetails]
	ADD
		[AdjustmentBalance] [decimal](18, 2) NULL
END
