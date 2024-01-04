using System.Collections;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using SyncfusionDataGrid.Pages;

namespace SyncfusionDataGrid;

public class CustomAdaptor : DataAdaptor
    {
        public List<OrderDetails> Orders { get; set; } = OrderDetails.GetAllRecords();
        public override async Task<Object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            IEnumerable GridData = Orders;
            IEnumerable DataSource = Orders;
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            if (dataManagerRequest.Sorted?.Count > 0) // perform Sorting
            {
                GridData = DataOperations.PerformSorting(GridData, dataManagerRequest.Sorted);
            }
            if (dataManagerRequest.Skip != 0)
            {
                GridData = DataOperations.PerformSkip(GridData, dataManagerRequest.Skip); //Paging
            }
            if (dataManagerRequest.Take != 0)
            {
                GridData = DataOperations.PerformTake(GridData, dataManagerRequest.Take);
            }
            IDictionary<string, object> aggregates = new Dictionary<string, object>();
            if (dataManagerRequest.Aggregates != null) // Aggregation
            {
                aggregates = DataUtil.PerformAggregation(DataSource, dataManagerRequest.Aggregates);
            }
            if (dataManagerRequest.Group != null && dataManagerRequest.Group.Any()) //Grouping
            {
                foreach (var group in dataManagerRequest.Group)
                {
                    GridData = DataUtil.Group<OrderDetails>(GridData, group, dataManagerRequest.Aggregates, 0, dataManagerRequest.GroupByFormatter);
                }
            }
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = GridData, Count = Orders.Count(), Aggregates = aggregates} : (object)GridData;
        }
        public override async Task<Object> InsertAsync(DataManager dataManager, object value, string key)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            Orders.Insert(0, value as OrderDetails);
            return value;
        }
        public override async Task<object> RemoveAsync(DataManager dataManager, object value, string keyField, string key)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            int data = (int)value;
            Orders.Remove(Orders.Where((Order) => Order.OrderID == data).FirstOrDefault());
            return value;
        }
        public override async Task<object> UpdateAsync(DataManager dataManager, object value, string keyField, string key)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            var val = (value as OrderDetails);
            var data = Orders.Where((Order) => Order.OrderID == val.OrderID).FirstOrDefault();
            if (data != null) {
                data.CustomerID = val.CustomerID;
                data.Freight = val.Freight;
                data.OrderDate = val.OrderDate;
                data.ShipCountry = val.ShipCountry;
            }
            return value;
        }
    }