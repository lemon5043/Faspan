DeliveryDrivers

2023/01/07
[v]�إ�readme
[v]����appsettings�s�u�r��
[v]EntityFrameworkCore.Tools,EntityFrameworkCore.SqlSever,EntityFrameworkCore.Desin�w�˧���
[v]�إ�EFModels
[v]�R��AppDbContext�����s�u�r��
[v]�bProgram.cs�����UAppDbContext
[v]�إ�DeliveryDriversController
[v]�ק�DeliveryDrivers:Index
2023/01/08
[v]�ק�DeliveryDriversController
    [v]�إ�Index.cshtml
    [v]�إ�Details.cshtml
    [v]�إ�Create.cshtml
    [v]�إ�Edit.cshtml
    [v]�إ�Delete.cshtml
[v]�إ�DeliveryRecrodesController
    [v]�إ�Index.cshtml
    [v]�إ�Details.cshtml
    [v]�إ�Edit.cshtml
        [v]Edit���O�ȧ����ݴ���
[v]�إ�DeliveryViolationRecordsController
    [v]�إ�Index
    [v]�إ�Details
[v]�إ�DeliveryCancellationRecordsController
    [v]�إ�Index
    [v]�إ�Details

    todo
[]DeliveryDriversController
    []Edit�L�k���`��ʡA��]����
[]DeliveryRecrodesController
    []�إ�Create
    []�إ�Delete
    []Details���������ƻݭn�H��p��
    []Edit�s�JDB
[]DeliveryViolationRecordsController
    []�إ�Create
    []�إ�Edit
    []�إ�Delete
[]DeliveryCancellationRecordsController
    []�إ�Create
    []�إ�Edit
    []�إ�Delete

Orders
order_ver0.1
1. ���ҫظm:
	1-1. Nuget�M��޲z: �s�WEFCore sqlserver�PEFCore tool(����6.0.12)
	1-2. ��sProgram�Pappsetting: �s�W�s�u�r��
2. entity.framework�ظm��¦����
	2-1. �s�WEFModels: �ϥ�EFCore Power tools�s�Wmodels
	2-2. �s�WOrdersController: �ϥ�EF�s�@��¦����

Order_ver0.1.1 ~ 0.1.2
1. ��s����:
	1-1. Nuget�M��޲z:�s�WEFCore Design(����6.0.12)
	1-2. ��s�s�u�r��
	1-3. ��sDatabase
	1-4. ���s�ظmEFModels: �ϥΫ��O�ظm

Order_ver0.2
1. ��sDatabase
2. ��sEFModels
3. �s�WOrderVM, OrderDetailVM
4. ����OrdersController: 
	4-1. �s�Windex: ��ư��˵��M��\��
5. �s�WView-Orders-index
6. �s�@�T�h���[�c:
	6-1. �s�WInfrastructures-ExtensionMethods-OrderExts

Order_ver0.2.1 ~ 0.2.2
1. �ק�OrdersController: �s�W���h�����(Order => Details)(������, ���i�˵�)
2. �ק�View-Orders-index: �ק�榡for���h���
 
Order_ver0.3(2023/01/10�Ȯɮi�ܪ���)
1. �s�WView-Orders-Details: �ȧ�ϥηsview�������N���h���e�{Details
2. �s�WView-Orders-ProductsDetail: �s�W���~����(������)
3. �ק�OrdersController: �s�Wmethod�H����detail�PProductsDetail����