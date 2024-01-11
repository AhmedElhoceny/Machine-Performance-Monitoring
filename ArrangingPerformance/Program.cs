using ArrangingPerformance;
using System.Data;

var processingFuns = new SystemFuns();

var fakeData = processingFuns.MakeFakeData(100000);

var result = new DataTable();

processingFuns.MonitorResourceUsage(async () => {
    result = await processingFuns.ArrangeDataTable(fakeData);
});

processingFuns.PrintDataTable(result);