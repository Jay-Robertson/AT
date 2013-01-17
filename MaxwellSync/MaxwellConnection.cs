using System;
using System.Data;
using System.Configuration;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;

namespace MaxwellSync
{
    public class MaxwellCustomer
    {
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
    }

    /// <summary>
    /// The MaxwellManager is used to import data from Maxwell into the AssetTracker system.
    /// </summary>
    public class MaxwellConnection : IDisposable
    {
        private readonly OdbcConnection _cx;

        public MaxwellConnection()
            : this(null)
        {

        }

        public MaxwellConnection(string maxwellConnectionString)
        {
            // AssetMaxwellInventoryItem: CINVM (Inventory Item Master)
            // AssetMaxwellJobHeader: CWIPH
            //  ""            Detail: CINRQ
            // Projet Manager File:   CSAMS
            // Job Class File:        CSCSY08 Description of codes related to jobs

            _cx = new OdbcConnection(
                maxwellConnectionString ??
                ConfigurationManager.AppSettings["MaxwellConnectionString"]
            );
            _cx.Open();
        }

        /*

        public string ImportInventory()
        {
            var updated = 0;
            var inserted = 0;
            using (var db = new AssetDBDataContext())
            {
                var odbc = new OdbcCommand("select * from CINVM", _cx);
                var dr = odbc.ExecuteReader();
                while (dr.Read())
                {
                    var co_code = dr["CO_CODE"] as string;
                    var inv_item_no = dr["INV_ITEM_NO"] as string;
                    var i = db.AssetMaxwellInventoryItems.FirstOrDefault(
                        x => x.CO_CODE == co_code &&
                             x.INV_ITEM_NO == inv_item_no);
                    if (null == i)
                    {
                        i = new AssetMaxwellInventoryItem
                        {
                            CO_CODE = co_code,
                            INV_ITEM_NO = inv_item_no,
                        };
                        db.AssetMaxwellInventoryItems.InsertOnSubmit(i);
                        inserted++;
                    }

                    i.INV_ITEM_DESC = dr["INV_ITEM_DESC"] as string;
                    i.ALT_ITEM_NO = dr["ALT_ITEM_NO"] as string;
                    i.NOT_USED_A = dr["NOT_USED_A"] as string;
                    i.NOT_USED_B = dr["NOT_USED_B"] as string;
                    i.UNIT_OF_MEASURE = dr["UNIT_OF_MEASURE"] as string;
                    i.TAXABLE_YN = dr["TAXABLE_YN"] as string;
                    i.UNIT_LIST_PRICE = dr["UNIT_LIST_PRICE"] as double?;
                    i.UNIT_COST = dr["UNIT_COST"] as double?;
                    i.ALT_COMM_BASIS = dr["ALT_COMM_BASIS"] as double?;
                    i.REORDER_PNT_TOT = dr["REORDER_PNT_TOT"] as double?;
                    i.LEAD_TIME = dr["LEAD_TIME"] as double?;
                    i.QTY_ON_HAND_TOT = dr["QTY_ON_HAND_TOT"] as double?;
                    i.QTY_COMMITD_TOT = dr["QTY_COMMITD_TOT"] as double?;
                    i.QTY_ON_ORDR_TOT = dr["QTY_ON_ORDR_TOT"] as double?;
                    i.QTY_SLD_YTD_TOT = dr["QTY_SLD_YTD_TOT"] as double?;
                    i.ITEM_CATEGORY = dr["ITEM_CATEGORY"] as string;
                    i.COST_METHOD = dr["COST_METHOD"] as string;
                    i.SALES_COMM_PCT = dr["SALES_COMM_PCT"] as double?;
                    i.COMMISSIONABLE = dr["COMMISSIONABLE"] as string;
                    i.EST_UNIT_COST = dr["EST_UNIT_COST"] as double?;
                    i.EST_MAN_HR_UNIT = dr["EST_MAN_HR_UNIT"] as double?;
                    i.EST_LABOR_CLASS = dr["EST_LABOR_CLASS"] as string;
                    i.UPC_NO = dr["UPC_NO"] as string;
                    i.EST_LABOR_RATE = dr["EST_LABOR_RATE"] as double?;
                    i.EST_MISC_1 = dr["EST_MISC_1"] as double?;
                    i.EST_MISC_2 = dr["EST_MISC_2"] as double?;
                    i.EST_MISC_3 = dr["EST_MISC_3"] as double?;
                    i.BILLING_CTGY = dr["BILLING_CTGY"] as string;

                    updated++;
                }

                db.SubmitChanges();
            }
            return String.Format("{0} inventory items modified ({1} new records)", updated, inserted);
        }

        */

        string ConvertMaxwellString(object input)
        {
            var a = input as string;
            if (String.IsNullOrWhiteSpace(a)) return null;
            return a.Trim();
        }

        public IList<MaxwellCustomer> FindCustomers()
        {
            var customers = new List<MaxwellCustomer>();
            using (var cmd = new OdbcCommand("select * from CCSMS", _cx))
            {
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        customers.Add(new MaxwellCustomer
                        {
                            CustomerNumber = ConvertMaxwellString(dr["CUST_NO"]),
                            CustomerName = ConvertMaxwellString(dr["CUST_NAME"]),
                            Address1 = ConvertMaxwellString(dr["CUST_ADDR_1"]),
                            Address2 = ConvertMaxwellString(dr["CUST_ADDR_2"]),
                            Address3 = ConvertMaxwellString(dr["CUST_ADDR_3"]),
                            ContactName = ConvertMaxwellString(dr["CUST_CONTACT"]),
                            PhoneNumber = ConvertMaxwellString(dr["CUST_PHONE_1"])
                        });
                    }
                }
            }
            return customers;
        }

        public void Dispose()
        {
            if (_cx.State == ConnectionState.Open)
            {
                _cx.Dispose();
            }
        }
    }
}