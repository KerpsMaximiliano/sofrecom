IF (NOT EXISTS (SELECT 1 FROM tiger.Recursos))
BEGIN
	INSERT INTO tiger.Recursos
	VALUES 
	(2774, 'Francisco Rosales', '1974-10-28', '2017-05-15', null, 'Developer', 'Microsoft', 'Junior'),
	(8794, 'Lucas Tadeo', '1984-06-11', '2016-10-05', null, 'Developer', 'Java', 'Semi-Senior'),
	(7848, 'Marcelo Rimpiano', '1987-03-06', '2015-06-01', null, 'Manager', 'Java', 'Senior')
END

IF (NOT EXISTS (SELECT 1 FROM rhpro.empleado))
BEGIN
	INSERT INTO rhpro.empleado
	VALUES 
	(2774, 1),
	(8794, 1),
	(7848, 1)
END

IF (NOT EXISTS (SELECT 1 FROM rhpro.emp_lic))
BEGIN
	INSERT INTO rhpro.emp_lic
	VALUES 
	(8794, '2018-01-01', '2018-02-01', 1)
END

IF (NOT EXISTS (SELECT 1 FROM rhpro.tipdia))
BEGIN
	INSERT INTO rhpro.tipdia
	VALUES 
	(1, 'Licences with salary'),
	(2, 'Licences no salary')
END
