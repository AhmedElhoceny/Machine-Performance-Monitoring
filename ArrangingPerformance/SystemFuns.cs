using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrangingPerformance
{
    public class SystemFuns
    {
        public async Task<DataTable> ArrangeDataTable(DataTable dataTable)
        {
            List<DataRow> rowsToMove = new List<DataRow>();
            var counter = 0;

            // iterate on dataTable rows
            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine($" the Setting Process DataTable row Number is --------> {++counter}");
                // get the row ParentResponseId
                var parentResponseId = row["ResponseId"].ToString();

                foreach (DataRow child in dataTable.Rows)
                {
                    var childParentResponseId = child["ParentResponseId"].ToString();

                    if (childParentResponseId == parentResponseId)
                    {
                        // add the child row to the list of rows to move
                        rowsToMove.Add(child);
                    }
                }
            }
            Console.Clear();

            counter = 0;

            // move the rows after the iteration is complete
            foreach (var rowToMove in rowsToMove)
            {
                Console.WriteLine($" the row Moving Rows Number is --------> {++counter }");
                // get the index of the parent row
                var parentRowIndex = dataTable.Rows.IndexOf(dataTable.Select($"ResponseId = '{rowToMove["ParentResponseId"]}'").FirstOrDefault());

                // get the index of the child row
                var childRowIndex = dataTable.Rows.IndexOf(rowToMove);

                // move the child row to be after the parent row
                dataTable = MoveRowToIndex(dataTable, childRowIndex, parentRowIndex + 1);
            }

            return dataTable;
        }

        public DataTable MoveRowToIndex(DataTable dataTable, int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex > dataTable.Rows.Count - 1)
                throw new ArgumentException("oldIndex");

            if (newIndex < 0 || newIndex > dataTable.Rows.Count - 1)
                throw new ArgumentException("newIndex");
            if (oldIndex == newIndex)
                return dataTable;

            var row = dataTable.Rows[oldIndex];

            // copy the row to a new row
            var newRow = dataTable.NewRow();
            newRow.ItemArray = row.ItemArray;

            dataTable.Rows.Remove(row);
            dataTable.Rows.InsertAt(newRow, newIndex);

            return dataTable;
        }

        public void MonitorResourceUsage(Action action)
        {
            using (var process = Process.GetCurrentProcess())
            {
                int processorCount = Environment.ProcessorCount;

                double cpuThreshold = 80.0 / processorCount;
                long memoryThreshold = 500 * 1024 * 1024;

                var thread = new Thread(() => action.Invoke());
                thread.Start();

                while (thread.IsAlive)
                {
                    if (process.TotalProcessorTime.TotalSeconds / process.TotalProcessorTime.TotalSeconds >= cpuThreshold)
                    {
                        Console.WriteLine($"High CPU usage detected! ---> {process.TotalProcessorTime.TotalSeconds / process.TotalProcessorTime.TotalSeconds}");
                    }

                    if (process.PrivateMemorySize64 >= memoryThreshold)
                    {
                        Console.WriteLine($"High memory usage detected! ---> {process.PrivateMemorySize64 / (1024 * 1024)}");
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        public DataTable MakeFakeData(int numberOfRows)
        {
            var counter = 0;
            var dataTable = new DataTable();
            var props = typeof(DemoModel).GetProperties();
            foreach (var item in props)
            {
                Console.WriteLine($" the Setting Process DataTable Column Number is --------> {++counter}");
                dataTable.Columns.Add(item.Name, item.PropertyType);
            }

            var fakeParent = new DemoModel()
            {
                Name = "Parent",
                ParentResponseId = Guid.Empty,
                ResponseId = Guid.NewGuid()
            };

            // clear the console to make it more readable
            Console.Clear();

            var parentRow = dataTable.NewRow();

            counter = 0;
            // make fakeParent row and take the values from fakeParent object
            foreach (var item in props)
            {
                parentRow[item.Name] = item.GetValue(fakeParent);
            }
            Console.WriteLine($" the Setting Process DataTable row Number is --------> {++counter}");

            dataTable.Rows.Add(parentRow);
            Console.Clear();

            counter = 0;
            // foreach row make fake data for each column
            for (int i = 0; i < numberOfRows; i++)
            {
                var row = dataTable.NewRow();
                foreach (var item in props)
                {
                    if (item.PropertyType == typeof(Guid))
                    {
                        if (item.Name == "ParentResponseId")
                            row[item.Name] = fakeParent.ResponseId;
                        else
                            row[item.Name] = Guid.NewGuid();
                    }
                    else if (item.PropertyType == typeof(string))
                    {
                        row[item.Name] = item.Name + "_Value";
                    }
                }
                Console.WriteLine($" the Setting Process DataTable row Number is --------> {++counter}");
                dataTable.Rows.Add(row);
            }
            Console.Clear();
            return dataTable;
        }

        public void PrintDataTable(DataTable dataTable)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.Write(column.ColumnName + "\t");
            }
            Console.WriteLine();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    Console.Write(row[column] + "\t");
                }
                Console.WriteLine();
            }
        }

    }
}
