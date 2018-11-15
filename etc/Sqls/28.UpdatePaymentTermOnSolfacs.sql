select * from app.Solfacs where PaymentTermId = 3
select * from app.Solfacs where PaymentTermId = 4

-- PaymentTermId = 1 - Pago a 30 días

--update app.Solfacs
--set PaymentTerm = 'Pago a 30 dias'
--where PaymentTerm is null

-- PaymentTermId = 3 - Pago a 45 dias
select * from app.Solfacs where id in (131, 233, 234, 309, 335, 427, 428, 542, 543, 544, 617, 659, 660, 661, 662 ,663, 665, 688, 690, 691, 782)

--update app.Solfacs
--set PaymentTerm = 'Pago a 45 dias'
--where id in (131, 233, 234, 309, 335, 427, 428, 542, 543, 544, 617, 659, 660, 661, 662 ,663, 665, 688, 690, 691, 782)

-- PaymentTermId = 4 - Pago a 60 días
select * from app.Solfacs where id in (49, 50, 60, 496, 524, 525, 526, 609, 610, 612, 670, 692, 695, 816, 817, 818)

--update app.Solfacs
--set PaymentTerm = 'Pago a 60 dias'
--where id in (49, 50, 60, 496, 524, 525, 526, 609, 610, 612, 670, 692, 695, 816, 817, 818)