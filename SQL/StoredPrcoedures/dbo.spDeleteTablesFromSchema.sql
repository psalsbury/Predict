create Procedure [dbo].DeleteTablesFromSchema
(
    @schemaName varchar(500)
)
As 
begin
    declare @constraintSchemaName nvarchar(128), @constraintTableName nvarchar(128),  @constraintName nvarchar(128)
    declare @sql nvarchar(max)
    -- delete FK first
    declare cur1 cursor for
    select distinct 
    CASE WHEN t2.[object_id] is NOT NULL  THEN  s2.name ELSE s.name END as SchemaName,
    CASE WHEN t2.[object_id] is NOT NULL  THEN  t2.name ELSE t.name END as TableName,
    CASE WHEN t2.[object_id] is NOT NULL  THEN  OBJECT_NAME(d2.constraint_object_id) ELSE OBJECT_NAME(d.constraint_object_id) END as ConstraintName
    from sys.objects t 
        inner join sys.schemas s 
            on t.[schema_id] = s.[schema_id]
        left join sys.foreign_key_columns d 
            on  d.parent_object_id = t.[object_id]
        left join sys.foreign_key_columns d2 
            on  d2.referenced_object_id = t.[object_id]
        inner join sys.objects t2 
            on  d2.parent_object_id = t2.[object_id]
        inner join sys.schemas s2 
            on  t2.[schema_id] = s2.[schema_id]
    WHERE t.[type]='U' 
        AND t2.[type]='U'
        AND t.is_ms_shipped = 0 
        AND t2.is_ms_shipped = 0 
        AND s.Name=@schemaName
    open cur1
    fetch next from cur1 into @constraintSchemaName, @constraintTableName, @constraintName
    while @@fetch_status = 0
    BEGIN
        set @sql ='ALTER TABLE ' + @constraintSchemaName + '.' + @constraintTableName+' DROP CONSTRAINT '+@constraintName+';'
        exec(@sql)
        fetch next from cur1 into @constraintSchemaName, @constraintTableName, @constraintName
    END
    close cur1
    deallocate cur1

    DECLARE @tableName nvarchar(128)
    declare cur2 cursor for
    select s.Name, p.Name
    from sys.objects p
        INNER JOIN sys.schemas s ON p.[schema_id] = s.[schema_id]
    WHERE p.[type]='U' and is_ms_shipped = 0 
    AND s.Name=@schemaName
    ORDER BY s.Name, p.Name
    open cur2

    fetch next from cur2 into @schemaName,@tableName
    while @@fetch_status = 0
    begin
        set @sql ='DROP TABLE ' + @schemaName + '.' + @tableName
        exec(@sql)
        fetch next from cur2 into @schemaName,@tableName
    end

    close cur2
    deallocate cur2

end
go