CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot_db;

USE cybersecurity_chatbot_db;

CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    reminder_date DATETIME NULL,
    is_completed BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);