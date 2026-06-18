using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Chat_Bot_Part2_POE
{
    // To handle all MySQL database actions for cybersecurity tasks
    public class TaskDatabaseService
    {
        // Connection string used to connect the C# app to your local MySQL database
        // Replace YOUR_PASSWORD with the root password you used in MySQL Workbench
        private string connectionString = "server=127.0.0.1;port=3306;user id=root;password=UrielEtanda@874;database=cybersecurity_chatbot_db;";

        // Adds a new task into the tasks table
        public void AddTask(CyberTask task)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // SQL query for inserting a task record
                string query = "INSERT INTO tasks (title, description, reminder_date, is_completed) VALUES (@title, @description, @reminderDate, @isCompleted)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add task values safely using parameters
                    command.Parameters.AddWithValue("@title", task.Title);
                    command.Parameters.AddWithValue("@description", task.Description);

                    // Store a reminder date if one exists, otherwise store NULL
                    if (task.ReminderDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@reminderDate", task.ReminderDate.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@reminderDate", DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@isCompleted", task.IsCompleted);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Reads all tasks from the database and returns them as a list
        public List<CyberTask> GetAllTasks()
        {
            List<CyberTask> tasks = new List<CyberTask>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Newest tasks are shown first
                string query = "SELECT id, title, description, reminder_date, is_completed FROM tasks ORDER BY created_at DESC";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Convert each database row into a CyberTask object
                        CyberTask task = new CyberTask
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Title = reader["title"].ToString(),
                            Description = reader["description"].ToString(),
                            ReminderDate = reader["reminder_date"] == DBNull.Value
                                ? null
                                : (DateTime?)Convert.ToDateTime(reader["reminder_date"]),
                            IsCompleted = Convert.ToBoolean(reader["is_completed"])
                        };

                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }




        // Updates a task in the database so it becomes completed
        public void CompleteTask(int taskId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        } //end of CompleteTask method




        // Deletes a task from the database using its ID
        public void DeleteTask(int taskId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM tasks WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        }//end of DeleteTask method






    }
}