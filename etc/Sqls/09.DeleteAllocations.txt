CREATE TRIGGER DeleteAllocations ON  app.allocations
FOR UPDATE
AS  
begin
    delete from app.allocations where releasedate = '0001-01-01 00:00:00.0000000'
end

