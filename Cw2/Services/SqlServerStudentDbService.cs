using Cw2.DTO;
using Cw2.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Cw2.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {

        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s16535;Integrated Security=True";

        public SqlServerStudentDbService()
        {
        }

        public Response EnrollStudent(EnrollStudentRequest request)
        {
            DateTime currentDate = DateTime.Now;

            using (var con = new SqlConnection(ConnectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE Name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    com.Transaction = tran;
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new Response("400 Bad Request", "Studia nie istnieją!");
                    }
                    int idstudies = (int)dr["IdStudy"];

                    dr.Close();

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@idstudies and semester=1";
                    com.Parameters.AddWithValue("idstudies", idstudies);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(Semester, IdStudy, StartDate) VALUES (@semester, @idstudy, @startdate)";
                        com.Parameters.AddWithValue("semester", 1);
                        com.Parameters.AddWithValue("idstudy", idstudies);
                        com.Parameters.AddWithValue("startdate", currentDate);

                        com.Transaction = tran;
                        com.ExecuteNonQuery();

                        dr.Close();
                        com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@idstudies AND semester=1";
                        com.Parameters.AddWithValue("idstudies", idstudies);
                        com.Transaction = tran;
                        dr = com.ExecuteReader();
                    }

                    int idenrollment = (int)dr["IdEnrollment"];

                    dr.Close();
                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @index";
                    com.Parameters.AddWithValue("index", request.IndexNumber);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new Response("400 Bad Request", "Student już istnieje!");
                    }
                    DateTime d = Convert.ToDateTime(request.BirthDate);
                    string convertedDate = d.Date.ToString("yyyy-MM-dd");

                    dr.Close();
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @fname, @lname, @bdate, @idenrollment)";
                    com.Parameters.AddWithValue("fname", request.FirstName);
                    com.Parameters.AddWithValue("lname", request.LastName);
                    com.Parameters.AddWithValue("bdate", convertedDate);
                    com.Parameters.AddWithValue("idenrollment", idenrollment);

                    com.ExecuteNonQuery();

                    tran.Commit();

                    var enrollment = new EnrollStudentResponse();
                    enrollment.Semester = 1;
                    enrollment.StartDate = currentDate;
                    enrollment.LastName = request.LastName;

                    return new Response("201 Created", "Dodano studenta!", enrollment);
                }
                catch (SqlException exc)
                {
                    Console.WriteLine(exc);
                    tran.Rollback();
                }
                return new Response("400 Bad Request", "Niepoprawne żądanie!");
            }
        }


        public Response PromoteStudents(PromoteStudentRequest request)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE Name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    com.Transaction = tran;
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new Response("404 Not Found", "Studia nie zostały odnalezione!");
                    }
                    dr.Close();

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment INNER JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy" +
                        " WHERE Studies.Name = @name AND Enrollment.Semester = 1";

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new Response("404 Not Found", "Wpis na studia nie został odnaleziony!");
                    }
                    dr.Close();

                    com.Parameters.Clear();
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "dbo.PromoteStudentsProcedure";
                    com.Parameters.Add(new SqlParameter("@Studies", request.Studies));
                    com.Parameters.Add(new SqlParameter("@Semester", request.Semester));

                    com.ExecuteNonQuery();
                    tran.Commit();

                    var response = new PromoteStudentResponse();
                    response.Studies = request.Studies;
                    response.Semester = request.Semester + 1;

                    return new Response("201 Created", "Promocja została wykonana pomyślnie!", response);
                }
                catch (SqlException exc)
                {
                    Console.WriteLine(exc);
                    tran.Rollback();
                }
                return new Response("400 Bad Request", "Niepoprawne żądanie!");
            }
        }

        public Response StudentExists(string indexNr)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @indexNr";
                com.Parameters.AddWithValue("indexNr", indexNr);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    return new Response("400 Bad Request", "Student o podanym indeksie nie istnieje!");
                }
                else 
                {
                    return new Response("200 Ok", "Student o podanym indeksie istnieje!");
                }
            }
        }
    }
}
