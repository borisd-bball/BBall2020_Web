using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using Spock.Davcna;
using System.IO;
using System.Web;
using System.Net;

namespace mr.bBall_Lib
{
    public class Porocila
    {
        public struct Vrstica
        {
            public Vrstica(string naziv, double davek95, double davek22, double znesek)
            {
                this.naziv = naziv;
                this.davek95 = davek95;
                this.davek22 = davek22;
                this.znesek = znesek;
            }
            public string naziv;
            public double davek95;
            public double davek22;
            public double znesek;
        }
        public static DataTable Get()
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select * from porocila", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.Fill(dt);
            }
            return dt;
        }
        public static DataTable Get(int id)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select * from porocila where id=@id", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.SelectCommand.Parameters.AddWithValue("id", id);
                a.Fill(dt);
            }
            return dt;
        }
        public static int Dodaj(DateTime datum_od, DateTime datum_do, string blagajna, bool racuni, bool blagajne)
        {
            int id = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comm = new SqlCommand("declare @id int;insert into porocila (datum_od,datum_do,racuni,blagajne,blagajna) values (@datum_od,@datum_do,@racuni,@blagajne,@blagajna);set @id=scope_identity();select @id;", conn, tran))
                        {
                            comm.Parameters.AddWithValue("datum_od", datum_od);
                            comm.Parameters.AddWithValue("datum_do", datum_do);
                            comm.Parameters.AddWithValue("racuni", racuni);
                            comm.Parameters.AddWithValue("blagajne", blagajne);
                            comm.Parameters.AddWithValue("blagajna", blagajna);
                            id = Convert.ToInt32(comm.ExecuteScalar());
                            tran.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return id;
        }
        public static void Popravi(int id, DateTime datum_od, DateTime datum_do, string blagajna, bool racuni, bool blagajne)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comm = new SqlCommand("update porocila set datum_od=@datum_od,datum_do=@datum_do,racuni=@racuni,blagajne=@blagajne,blagajna=@blagajna where id=@id", conn, tran))
                        {
                            comm.Parameters.AddWithValue("id", id);
                            comm.Parameters.AddWithValue("datum_od", datum_od);
                            comm.Parameters.AddWithValue("datum_do", datum_do);
                            comm.Parameters.AddWithValue("racuni", racuni);
                            comm.Parameters.AddWithValue("blagajne", blagajne);
                            comm.Parameters.AddWithValue("blagajna", blagajna);
                            comm.ExecuteNonQuery();
                            tran.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public static void Brisi(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comm = new SqlCommand("delete from porocila where id=@id", conn, tran))
                        {
                            comm.Parameters.AddWithValue("id", id);
                            comm.ExecuteNonQuery();
                            tran.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
