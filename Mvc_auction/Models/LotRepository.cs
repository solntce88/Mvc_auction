using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Mvc_auction.Models.DB;

namespace Mvc_auction.Models
{
    public class LotRepository
    {
        string connString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

        public void UpdateLot(Lot lot)
        {
            string procedure = "P_ULot";
            SqlParameter lot_id = new SqlParameter("@lot_id", lot.Id);
            SqlParameter name = new SqlParameter("@name", lot.Name);
            SqlParameter description = new SqlParameter("@description", lot.Description);
            SqlParameter price = new SqlParameter("@price", lot.Price);
           // SqlParameter startTime = new SqlParameter("@startTime", lot.StartTime);
           // SqlParameter dateEnd = new SqlParameter("@dateEnd", lot.DateEnd);
            SqlParameter picture = new SqlParameter("@picture", lot.Picture);
           // SqlParameter owner_id = new SqlParameter("@owner_id", lot.Owner_id);
            SqlParameter category_id = new SqlParameter("@category_id", lot.Category_id);

            SqlParameter[] col_param = { lot_id, price, description, name, picture, category_id }; //, dateEnd, startTime , owner_id

            ADO_db.ExeProcedure(connString, procedure, col_param);
        }        //
        public Lot GetLot(int id)
        {
            string qSelect = "SELECT [name],[description],[price],[customer_id],[startTime],[dateEnd],[picture],[owner_id],[category_id] FROM [ASPNET_appDB].[dbo].[Lot] Where [lot_id]=@id";
            SqlParameter param = new SqlParameter("@id", id);

            Lot lot = new Lot();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(qSelect, conn);
                cmd.Parameters.Add(param);
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //    lot = ReadLot(reader);
                //    return lot.ElementAt(0);
                //}
                #region Reader
                using (reader)
                {
                    while (reader.Read())
                    {

                        lot.Id = id;

                        try
                        { lot.Name = (string)reader["name"]; }
                        catch (InvalidCastException) { lot.Name = "noName"; }

                        try
                        { lot.Description = (string)reader["description"]; }
                        catch (InvalidCastException) { lot.Description = "noDescription"; }

                        try
                        { lot.Price = (double)reader["price"]; }
                        catch (InvalidCastException) { lot.Price = 0; }

                        try
                        { lot.Picture = (string)reader["picture"]; }
                        catch (InvalidCastException) { lot.Picture = "/Content/images/noImage.png"; }

                        try
                        { lot.Owner_id = (int)reader["owner_id"]; }
                        catch (InvalidCastException) { lot.Owner_id = 0; }

                        try
                        { lot.Customer_id = (int)reader["customer_id"]; }
                        catch (InvalidCastException) { lot.Customer_id = 0; }

                        try
                        { lot.Category_id = (int)reader["category_id"]; }
                        catch (InvalidCastException) { lot.Category_id = 0; }

                        try
                        { lot.StartTime = (DateTime)reader["startTime"]; }
                        catch (InvalidCastException) { lot.StartTime = DateTime.Now; }

                        try
                        { lot.DateEnd = (DateTime)reader["dateEnd"]; }
                        catch (InvalidCastException) { lot.DateEnd = DateTime.Now; }
                    }
                }
            }
            return lot;

                #endregion
        }
       /* protected List<Lot> ReadLot(SqlDataReader reader)
        {
            List<Lot> lots = new List<Lot>();
            using (reader)
            {
                while (reader.Read())
                {
                    Lot tmpLot = new Lot();
                    tmpLot.Id = (int)reader["lot_id"];

                    try
                    { tmpLot.Name = (string)reader["name"]; }
                    catch (InvalidCastException) { tmpLot.Name = "noName"; }

                    try
                    { tmpLot.Description = (string)reader["description"]; }
                    catch (InvalidCastException) { tmpLot.Description = "noDescription"; }

                    try
                    { tmpLot.Price = (double)reader["price"]; }
                    catch (InvalidCastException) { tmpLot.Price = 0; }

                    try
                    { tmpLot.Picture = (string)reader["picture"]; }
                    catch (InvalidCastException) { tmpLot.Picture = "/Content/images/noPicture.png"; }

                    try
                    { tmpLot.Owner_id = (int)reader["owner_id"]; }
                    catch (InvalidCastException) { tmpLot.Owner_id = 0; }

                    try
                    { tmpLot.Customer_id = (int)reader["customer_id"]; }
                    catch (InvalidCastException) { tmpLot.Customer_id = 0; }

                    try
                    { tmpLot.Category_id = (int)reader["category_id"]; }
                    catch (InvalidCastException) { tmpLot.Category_id = 0; }

                    try
                    { tmpLot.StartTime = (DateTime)reader["startTime"]; }
                    catch (InvalidCastException) { tmpLot.StartTime = DateTime.Now; }

                    try
                    { tmpLot.DateEnd = (DateTime)reader["dateEnd"]; }
                    catch (InvalidCastException) { tmpLot.DateEnd = DateTime.Now; }
                    lots.Add(tmpLot);
                }
            }
            return lots;
        }
        */ 
        // cписок лотов
        public List<Lot> GetInactiveLot()
        {
            //IEnumerable<Lot> lotsToMail = GetLots().Where(c => c.DateEnd < DateTime.Now.AddMinutes(5));
            //List<Lot> list = lotsToMail.ToList<Lot>();
            //list.Sort(IComparer<Lot>);
            //return list;
            //string qSelect = "SELECT [lot_id],[name],[description],[price],[customer_id],[startTime],[dateEnd],[picture],[owner_id],[category_id] FROM [ASPNET_appDB].[dbo].[Lot] where [dateEnd]>@dateTimeBegin and [dateEnd]>";
            string qSelect = "SELECT [lot_id],[name],[description],[price],[customer_id],[startTime],[dateEnd],[picture],[owner_id],[category_id] FROM [ASPNET_appDB].[dbo].[Lot] where [dateEnd]>@begin and [dateEnd]<@end";
            SqlParameter begin = new SqlParameter("@begin", DateTime.Now);
            SqlParameter end = new SqlParameter("@end", DateTime.Now.AddMinutes(5+1));
            SqlParameter[] parameters = { begin,end };
            return GetLots(qSelect, parameters);
        }
        public List<Lot> GetAllLots()
        {
            string qSelect = "SELECT [lot_id],[name],[description],[price],[customer_id],[startTime],[dateEnd],[picture],[owner_id],[category_id] FROM [ASPNET_appDB].[dbo].[Lot] where [dateEnd]>@dateTime";
            SqlParameter param1 = new SqlParameter("@dateTime", DateTime.Now);
            SqlParameter [] parameters ={param1};
           
            return GetLots(qSelect, parameters); 
        }
        public List<Lot> GetLots(string qSelect, SqlParameter [] param)
        {
            List<Lot> lots = new List<Lot>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(qSelect, conn);
                cmd.Parameters.AddRange(param);
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        Lot tmpLot = new Lot();
                        tmpLot.Id = (int)reader["lot_id"];

                        try
                        { tmpLot.Name = (string)reader["name"]; }
                        catch (InvalidCastException) { tmpLot.Name = "noName"; }

                        try
                        { tmpLot.Description = (string)reader["description"]; }
                        catch (InvalidCastException) { tmpLot.Description = "noDescription"; }

                        try
                        { tmpLot.Price = (double)reader["price"]; }
                        catch (InvalidCastException) { tmpLot.Price = 0; }

                        try
                        { tmpLot.Picture = (string)reader["picture"]; }
                        catch (InvalidCastException) { tmpLot.Picture = "/Content/images/noPicture.png"; }

                        try
                        { tmpLot.Owner_id = (int)reader["owner_id"]; }
                        catch (InvalidCastException) { tmpLot.Owner_id = 0; }

                        try
                        { tmpLot.Customer_id = (int)reader["customer_id"]; }
                        catch (InvalidCastException) { tmpLot.Customer_id = 0; }

                        try
                        { tmpLot.Category_id = (int)reader["category_id"]; }
                        catch (InvalidCastException) { tmpLot.Category_id = 0; }

                        try
                        { tmpLot.StartTime = (DateTime)reader["startTime"]; }
                        catch (InvalidCastException) { tmpLot.StartTime = DateTime.Now; }

                        try
                        { tmpLot.DateEnd = (DateTime)reader["dateEnd"]; }
                        catch (InvalidCastException) { tmpLot.DateEnd = DateTime.Now; }
                        lots.Add(tmpLot);
                    }
                }
                conn.Close();
            }
            return lots;
        }
        public bool DeleteLot(int id)
        {
            try
            {
                string procedure = "P_DLot";
                SqlParameter param = new SqlParameter("@lot_id", id);
                ADO_db.ExeProcedure(connString, procedure, param);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return false;
        }
        public bool DeleteUserLots(int user_id)
        {
            try
            {
                string procedure = "P_DUserLots";
                SqlParameter param = new SqlParameter("@user_id", user_id);
                ADO_db.ExeProcedure(connString, procedure, param);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return false;
        }
        public bool InsertLot(Lot lot)
        {
            bool result = false;
            try
            {
                string procedure = "P_ILot";

                SqlParameter price = new SqlParameter("@price", lot.Price);
                SqlParameter description = new SqlParameter("@description", lot.Description);
                SqlParameter name = new SqlParameter("@name", lot.Name);
                SqlParameter picture = new SqlParameter("@picture", lot.Picture);
                SqlParameter category_id = new SqlParameter("@category_id", lot.Category_id);
                SqlParameter owner_id = new SqlParameter("@owner_id", lot.Owner_id);
                SqlParameter dateEnd = new SqlParameter("@dateEnd", lot.DateEnd);
                SqlParameter startTime = new SqlParameter("@startTime", lot.StartTime);
                SqlParameter[] col_param = { price, description, name, picture, category_id, owner_id, dateEnd, startTime };
                ADO_db.ExeProcedure(connString, procedure, col_param);
                result = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public double GetPrice(int id)
        {
            string qSelect = "SELECT [price] FROM [ASPNET_appDB].[dbo].[Lot] Where [lot_id]=@id";
            SqlParameter param = new SqlParameter("@id", id);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(qSelect, conn);
                cmd.Parameters.Add(param);
                cmd.Connection.Open();
                 
                var price=(double)cmd.ExecuteScalar();
                return price;
            }
        }
        public bool SetPrice(int id, double price,int newCustomer_id=0)
        {
            bool result = false;
            string procedure = "P_UCustomerLot";
            SqlParameter p_Id = new SqlParameter("@lot_id", id);
            SqlParameter p_Price = new SqlParameter("@price",price);
            SqlParameter p_Customer = new SqlParameter("@customer_id", newCustomer_id);
            SqlParameter[] parameters = { p_Id, p_Price, p_Customer };
            try
            {
                ADO_db.ExeProcedure(connString, procedure, parameters);
                result = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
    }
}